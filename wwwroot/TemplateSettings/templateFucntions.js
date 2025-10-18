
  
// Global variables
let rowIndex = 1;
let excelHeaders = [];
let detectedHeaderRow = 0;
let databaseFields = [];
let templateIdToDelete = null;

// DOM elements
const elements = {
    packingForm: document.getElementById('packingForm'),
    excelFile: document.getElementById('excelFile'),
    headerInfo: document.getElementById('headerInfo'),
    headerRowText: document.getElementById('headerRowText'),
    mappingsContainer: document.getElementById('mappingsContainer'),
    addRowBtn: document.getElementById('addRowBtn'),
    submitBtn: document.getElementById('submitBtn'),
    backBtn: document.getElementById('backBtn'),
    templateList: document.getElementById('templateList'),
    templateContainer: document.getElementById('templateContainer'),
    templateName: document.getElementById('templateName'),
    setActive: document.getElementById('setActive')
};

// Initialize the app
document.addEventListener('DOMContentLoaded', function() {
    initializeEventListeners();
    loadDatabaseColumns();
});

// Initialize event listeners
function initializeEventListeners() {
    elements.excelFile.addEventListener('change', handleFileChange);
    elements.addRowBtn.addEventListener('click', addMappingRow);
    elements.packingForm.addEventListener('submit', handleFormSubmit);
    elements.backBtn.addEventListener('click', showTemplateList);
    document.getElementById('confirmDeleteBtn').addEventListener('click', handleDeleteConfirmation);
}

// Handle file change
function handleFileChange(e) {
    const file = e.target.files[0];
    if (!file) return;

    // Validate file type
    const validTypes = ['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.ms-excel', 'text/csv'];
    if (!validTypes.includes(file.type) && !file.name.endsWith('.xlsx') && !file.name.endsWith('.csv')) {
        showAlert('Please select a valid Excel or CSV file.', 'danger');
        e.target.value = '';
        return;
    }

    const reader = new FileReader();
    reader.onload = function(event) {
        try {
            const data = new Uint8Array(event.target.result);
            const workbook = XLSX.read(data, {type: 'array'});
            const firstSheetName = workbook.SheetNames[0];
            const worksheet = workbook.Sheets[firstSheetName];
            const jsonData = XLSX.utils.sheet_to_json(worksheet, {header:1, defval: ''});
            
            if (jsonData.length > 0) {
                const headerInfo = detectHeaderRow(jsonData);
                detectedHeaderRow = headerInfo.rowIndex;
                excelHeaders = headerInfo.headers;
                
                // Update UI
                elements.headerRowText.textContent = `Row ${detectedHeaderRow + 1} (index: ${detectedHeaderRow})`;
                elements.headerInfo.style.display = 'block';
                
                updateExcelFieldOptions(excelHeaders);
            } else {
                showAlert('The uploaded file appears to be empty or invalid.', 'warning');
            }
        } catch (error) {
            console.error('Error processing file:', error);
            showAlert('Error processing the uploaded file. Please try again.', 'danger');
        }
    };
    
    reader.onerror = function() {
        showAlert('Error reading the file. Please try again.', 'danger');
    };
    
    reader.readAsArrayBuffer(file);
}

// Detect header row in Excel data
function detectHeaderRow(jsonData) {
    // Strategy 1: Look for the row with the most non-empty cells
    let maxNonEmpty = 0;
    let candidateRow = 0;
    
    // Strategy 2: Look for common header patterns
    const commonHeaders = ["name", "id", "number", "date", "quantity", "description", "price", "amount"];
    
    for (let row = 0; row < Math.min(10, jsonData.length); row++) {
        let nonEmptyCount = 0;
        let headerMatchCount = 0;
        
        for (let col = 0; col < jsonData[row].length; col++) {
            const cellValue = jsonData[row][col] ? jsonData[row][col].toString().toLowerCase().trim() : "";
            
            if (cellValue !== "") {
                nonEmptyCount++;
                
                // Check if this cell matches common header patterns
                if (commonHeaders.some(h => cellValue.includes(h))) {
                    headerMatchCount++;
                }
            }
        }
        
        // If this row has more non-empty cells than previous candidates, or more header matches
        if (nonEmptyCount > maxNonEmpty || 
            (nonEmptyCount >= 3 && headerMatchCount > 0 && row > 0)) {
            maxNonEmpty = nonEmptyCount;
            candidateRow = row;
        }
    }
    
    // Return the detected row and its headers
    return {
        rowIndex: candidateRow,
        headers: jsonData[candidateRow] || []
    };
}

// Load database columns
async function loadDatabaseColumns() {
    try {
        const response = await fetch('/api/packaging-list/columns'); 
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        console.log("db",result) 
        if (result.success) {
            databaseFields = result.data;
            updateDatabaseFieldOptions();
        } else {
            console.error('Failed to load database columns:', result.message);
            showAlert('Error loading database columns: ' + result.message, 'danger');
        }
    } catch (error) {
        console.error('Error loading database columns:', error);
        showAlert('Error loading database columns: ' + error.message, 'danger');
    }
}

// Update Excel field options
function updateExcelFieldOptions(headers) {
    console.log("Updating Excel field options with headers:", headers);
    
    const dropdowns = document.querySelectorAll('.excel-field');
    dropdowns.forEach(dropdown => {
        // Clear existing options except the first one
        while (dropdown.options.length > 1) {
            dropdown.remove(1);
        }
        
        // Add new options
        if (headers && headers.length > 0) {
            headers.forEach(header => {
                const option = document.createElement('option');
                option.value = header;
                option.textContent = header;
                dropdown.appendChild(option);
                console.log(`Added option: ${header}`);
            });
        }
    });
}

// Update Excel field options for a specific dropdown
function updateExcelFieldOptionsForDropdown(dropdown, headers) {
    console.log("Updating Excel field options for dropdown with headers:", headers);
    
    // Clear existing options except the first one
    while (dropdown.options.length > 1) {
        dropdown.remove(1);
    }
    
    // Add new options
    if (headers && headers.length > 0) {
        headers.forEach(header => {
            const option = document.createElement('option');
            option.value = header;
            option.textContent = header;
            dropdown.appendChild(option);
            console.log(`Added option to dropdown: ${header}`);
        });
    }
}

function updateDatabaseFieldOptions() {
    const dropdowns = document.querySelectorAll('.form-field');
    dropdowns.forEach(dropdown => {
        // Clear existing options
        while (dropdown.options.length > 0) {
            dropdown.remove(0);
        }
        
        // Add default option
        const defaultOption = document.createElement('option');
        defaultOption.value = "";
        defaultOption.textContent = "Select a field";
        dropdown.appendChild(defaultOption);
        
        // Add database field options
        databaseFields.forEach(field => {
            const option = document.createElement('option');
            option.value = field;
            
            // Format the field name for display (convert PascalCase to Title Case)
            const displayName = field.replace(/([A-Z])/g, ' $1')
                .replace(/^./, str => str.toUpperCase());
            option.textContent = displayName;
            
            dropdown.appendChild(option);
        });
    });
}

// Add a new mapping row
function addMappingRow() {
    const container = elements.mappingsContainer;
    const newRow = document.createElement('div');
    newRow.className = 'mapping-row';
    newRow.setAttribute('data-index', rowIndex);
    
    newRow.innerHTML = `
        <div class="row align-items-end">
            <div class="col-md-5">
                <label class="form-label">Excel Mail Merge Field</label>
                <select class="form-select excel-field" required>
                    <option value="">Select a field</option>
                </select>
                <div class="invalid-feedback">Please select an Excel field.</div>
            </div>
            <div class="col-md-5">
                <label class="form-label">Database Field</label>
                <select class="form-select form-field" required>
                    <option value="">Select a field</option>
                </select>
                <div class="invalid-feedback">Please select a database field.</div>
            </div>
            <div class="col-md-2">
                <div class="form-check">
                    <input class="form-check-input recurring-check" type="checkbox" id="recurring${rowIndex}">
                    <label class="form-check-label" for="recurring${rowIndex}">
                        Recurring data
                    </label>
                </div>
            </div>
            <div class="col-md-1">
                <button type="button" class="btn btn-remove btn-sm w-100" onclick="removeRow(this)">Remove</button>
            </div>
        </div>
    `;
    
    container.appendChild(newRow);
    
    // Update the excel field options for the new row
    const newExcelDropdown = newRow.querySelector('.excel-field');
    updateExcelFieldOptionsForDropdown(newExcelDropdown, excelHeaders);
    
    // Update the database field options for the new row
    const newDbDropdown = newRow.querySelector('.form-field');
    updateDatabaseFieldOptionsForDropdown(newDbDropdown);
    
    rowIndex++;
}

// Update database field options for a specific dropdown
function updateDatabaseFieldOptionsForDropdown(dropdown) {
    // Clear existing options
    while (dropdown.options.length > 0) {
        dropdown.remove(0);
    }
    
    // Add default option
    const defaultOption = document.createElement('option');
    defaultOption.value = "";
    defaultOption.textContent = "Select a field";
    dropdown.appendChild(defaultOption);
    
    // Add database field options
    databaseFields.forEach(field => {
        const option = document.createElement('option');
        option.value = field;
        
        // Format the field name for display
        const displayName = field.replace(/([A-Z])/g, ' $1')
            .replace(/^./, str => str.toUpperCase());
        option.textContent = displayName;
        
        dropdown.appendChild(option);
    });
}

// Remove a mapping row
function removeRow(btn) {
    btn.closest('.mapping-row').remove();
}

// Handle form submission
async function handleFormSubmit(e) {
    e.preventDefault();
    
    // Validate form
    if (!elements.packingForm.checkValidity()) {
        e.stopPropagation();
        elements.packingForm.classList.add('was-validated');
        return;
    }
    
    // Check if file is selected (only required for new templates)
    const templateIdInput = document.getElementById('templateId');
    const isUpdate = templateIdInput && templateIdInput.value;
    
    if (!isUpdate && !elements.excelFile.files.length) {
        showAlert('Please select an Excel or CSV file.', 'warning');
        return;
    }
    
    // Check if at least one mapping is defined
    const mappingRows = document.querySelectorAll('.mapping-row');
    let hasValidMapping = false;
    
    mappingRows.forEach(row => {
        const excelField = row.querySelector('.excel-field').value;
        const formField = row.querySelector('.form-field').value;
        
        if (excelField && formField) {
            hasValidMapping = true;
        }
    });
    
    if (!hasValidMapping) {
        showAlert('Please define at least one field mapping.', 'warning');
        return;
    }
    
    // Prepare form data
    const templateName = elements.templateName.value;
    const excelFile = elements.excelFile.files[0];
    const setActive = elements.setActive.checked;
    
    const formData = new FormData();
    formData.append('Name', templateName);
    formData.append('IsActive', setActive);
    
    // Add file if it's a new template or if file is selected for update
    if (!isUpdate || (isUpdate && excelFile)) {
        formData.append('File', excelFile);
    }
    
    // Add template ID if it's an update
    if (isUpdate) {
        formData.append('Id', templateIdInput.value);
    }
    
    // Add mappings
    const mappings = [];
    mappingRows.forEach(row => {
        const excelField = row.querySelector('.excel-field').value;
        const formField = row.querySelector('.form-field').value;
        const recurring = row.querySelector('.recurring-check').checked;
        
        if (excelField && formField) {
            mappings.push({ 
                excelField: excelField, 
                databaseField: formField, 
                recurring: recurring 
            });
        }
    });
    
    mappings.forEach((mapping, index) => {
        formData.append(`Mappings[${index}].ExcelColumn`, mapping.excelField);
        formData.append(`Mappings[${index}].DatabaseField`, mapping.databaseField);
        formData.append(`Mappings[${index}].IsRecurring`, mapping.recurring);
    });
    
    // Show loading state
    elements.submitBtn.disabled = true;
    elements.submitBtn.classList.add('btn-loading');
    elements.submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> ' + (isUpdate ? 'Updating...' : 'Submitting...');
    
    try {
        const url = isUpdate ? `/api/template/${templateIdInput.value}` : '/api/template';
        const method = isUpdate ? 'PUT' : 'POST';
        
        const response = await fetch(url, {
            method: method,
            body: formData
        });
        
        const result = await response.json();
        
        if (response.ok && result.success) {
            showAlert(isUpdate ? 'Template updated successfully!' : 'Template created successfully!', 'success');
            showTemplateList();
        } else {
            // Display error messages
            const errorMessage = result.errors && result.errors.length > 0 
                ? result.errors.join(', ') 
                : result.message || (isUpdate ? 'Failed to update template' : 'Failed to create template');
            
            showAlert(`Error: ${errorMessage}`, 'danger');
        }
    } catch (error) {
        console.error('Error submitting form:', error);
        showAlert(isUpdate ? 'Error updating template. Please try again.' : 'Error creating template. Please try again.', 'danger');
    } finally {
        // Reset loading state
        elements.submitBtn.disabled = false;
        elements.submitBtn.classList.remove('btn-loading');
        elements.submitBtn.textContent = isUpdate ? 'Update Template' : 'Submit';
    }
}

// Show template list
function showTemplateList() {
    elements.packingForm.style.display = 'none';
    elements.templateList.style.display = 'block';
    
    // Show loading spinner
    elements.templateContainer.innerHTML = `
        <div class="d-flex justify-content-center my-5">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    `;
    
    // Fetch templates
    fetch('/api/template')
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(result => {
            if (result.success) {
                console.log("templates",result.data)
                displayTemplates(result.data);
            } else {
                elements.templateContainer.innerHTML = `
                    <div class="alert alert-danger">
                        Error loading templates: ${result.message}
                    </div>
                `;
            }
        })
        .catch(error => {
            console.error('Error loading templates:', error);
            elements.templateContainer.innerHTML = `
                <div class="alert alert-danger">
                    Error loading templates: ${error.message}
                </div>
            `;
        });
}

// Display templates in the UI
function displayTemplates(templates) {
    if (!templates || templates.length === 0) {
        elements.templateContainer.innerHTML = `
            <div class="alert alert-info">
                No templates found. Create a new template to get started.
            </div>
        `;
        return;
    }
    
    elements.templateContainer.innerHTML = '';
    
    templates.forEach(template => {
        const templateItem = document.createElement('div');
        templateItem.className = 'template-item';
        
        // Check if the template has the expected properties
        const isActive = template.isActive !== undefined ? template.isActive : template.IsActive;
        const name = template.name !== undefined ? template.name : template.Name;
        const createdAt = template.createdAt !== undefined ? template.createdAt : template.CreatedAt;
        const id = template.id !== undefined ? template.id : template.Id;
        
        const isActiveBadge = isActive ? 
            '<span class="badge bg-success">Active</span>' : 
            '<span class="badge bg-secondary">Inactive</span>';
        
        // Create template info section
        const infoSection = document.createElement('div');
        infoSection.innerHTML = `
            <strong>${name}</strong>
            ${isActiveBadge}
            <div class="text-muted small">Created: ${new Date(createdAt).toLocaleString()}</div>
        `;
        
        // Create buttons container
        const buttonsContainer = document.createElement('div');
        buttonsContainer.className = 'd-flex gap-2';
        
        // Create download button
        // const downloadBtn = document.createElement('button');
        // downloadBtn.className = 'btn btn-primary btn-sm';
        // downloadBtn.textContent = 'Download with Data';
        // downloadBtn.addEventListener('click', () => {
        //     downloadTemplate(id);
        // });

        // In your displayTemplates function, update the download button event listener
          // In your displayTemplates function, update the download button creation
// const downloadBtn = document.createElement('button');
// downloadBtn.className = 'btn btn-primary btn-sm me-2';
// downloadBtn.textContent = 'Download with Data';
// downloadBtn.addEventListener('click', () => {
//     downloadExcelWithTemplateData(id);  // Make sure this is calling the correct function
// });
        
        // Create edit button
        const editBtn = document.createElement('button');
        editBtn.className = 'btn btn-outline-secondary btn-sm';
        editBtn.textContent = 'Edit';
        editBtn.addEventListener('click', () => {
            editTemplate(id);
        });
        
        // Create delete button
        const deleteBtn = document.createElement('button');
        deleteBtn.className = 'btn btn-danger btn-sm';
        deleteBtn.innerHTML = '<i class="bi bi-trash"></i> Delete';
        deleteBtn.addEventListener('click', () => {
            confirmDeleteTemplate(id, name);
        });
        
        // Add buttons to container
        // buttonsContainer.appendChild(downloadBtn);
        buttonsContainer.appendChild(editBtn);
        buttonsContainer.appendChild(deleteBtn);
        
        // Add sections to template item
        templateItem.appendChild(infoSection);
        templateItem.appendChild(buttonsContainer);
        
        elements.templateContainer.appendChild(templateItem);
    });
}

// Show delete confirmation modal
function confirmDeleteTemplate(templateId, templateName) {
    templateIdToDelete = templateId;
    document.getElementById('templateNameToDelete').textContent = templateName;
    
    const modal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
    modal.show();
}

// Handle delete confirmation
function handleDeleteConfirmation() {
    if (templateIdToDelete) {
        deleteTemplate(templateIdToDelete);
        
        // Hide the modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('deleteConfirmModal'));
        modal.hide();
    }
}

// Delete template function
async function deleteTemplate(templateId) {
    try {
        showAlert('Deleting template...', 'info');
        
        const response = await fetch(`/api/template/${templateId}`, {
            method: 'DELETE'
        });
        
        const result = await response.json();
        
        if (response.ok && result.success) {
            showAlert('Template deleted successfully!', 'success');
            // Refresh the template list
            showTemplateList();
        } else {
            const errorMessage = result.message || 'Failed to delete template';
            showAlert(`Error: ${errorMessage}`, 'danger');
        }
    } catch (error) {
        console.error('Error deleting template:', error);
        showAlert('Error deleting template. Please try again.', 'danger');
    }
}

// Make functions globally accessible
window.downloadTemplate = function(templateId) {
    // Ensure templateId is a string
    const id = String(templateId);
    
    // Show loading indicator
    showAlert('Preparing your download...', 'info');
    
    fetch(`/api/template/${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.blob();
        })
        .then(blob => {
            // Create a download link
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            
            // Get the template name to use as filename
            fetch(`/api/template/${id}`)
                .then(response => response.json())
                .then(result => {
                    if (result.success) {
                        const templateName = result.data.name || result.data.Name || 'Template';
                        a.download = `${templateName}_WithData.xlsx`;
                    } else {
                        a.download = 'TemplateWithData.xlsx';
                    }
                    
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    
                    showAlert('Download completed successfully!', 'success');
                })
                .catch(error => {
                    console.error('Error getting template name:', error);
                    a.download = 'TemplateWithData.xlsx';
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    
                    showAlert('Download completed!', 'success');
                });
        })
        .catch(error => {
            console.error('Error downloading template:', error);
            showAlert('Error downloading template. Please try again.', 'danger');
        });
};

window.editTemplate = function(templateId) {
    // Ensure templateId is a string
    const id = String(templateId);
    
    // Show loading indicator
    showAlert('Loading template data...', 'info');
    
    fetch(`/api/template/${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(result => {
            if (result.success) {
                const template = result.data;
                
                // Switch to form view
                elements.templateList.style.display = 'none';
                elements.packingForm.style.display = 'block';
                
                // Populate form with template data
                elements.templateName.value = template.name || template.Name || '';
                elements.setActive.checked = template.isActive || template.IsActive || false;
                
                // Clear existing mappings
                elements.mappingsContainer.innerHTML = '';
                
                // Get mappings with proper property name handling
                const mappings = template.mappings || template.Mappings || [];
                console.log("Mappings:", mappings);
                
                // Extract the Excel columns from the mappings
                const excelColumns = mappings.map(mapping => mapping.excelColumn || mapping.ExcelColumn || '');
                console.log("Excel columns:", excelColumns);
                
                // Set the global excelHeaders array
                excelHeaders = excelColumns;
                
                // Ensure we have at least one mapping row in the container
                if (elements.mappingsContainer.children.length === 0) {
                    // Create a new mapping row
                    const newRow = document.createElement('div');
                    newRow.className = 'mapping-row';
                    newRow.setAttribute('data-index', '0');
                    
                    newRow.innerHTML = `
                        <div class="row align-items-end">
                            <div class="col-md-5">
                                <label class="form-label">Excel Mail Merge Field</label>
                                <select class="form-select excel-field" required>
                                    <option value="">Select a field</option>
                                </select>
                                <div class="invalid-feedback">Please select an Excel field.</div>
                            </div>
                            <div class="col-md-5">
                                <label class="form-label">Database Field</label>
                                <select class="form-select form-field" required>
                                    <option value="">Select a field</option>
                                </select>
                                <div class="invalid-feedback">Please select a database field.</div>
                            </div>
                            <div class="col-md-2">
                                <div class="form-check">
                                    <input class="form-check-input recurring-check" type="checkbox" id="recurring0">
                                    <label class="form-check-label" for="recurring0">
                                        Recurring data
                                    </label>
                                </div>
                            </div>
                        </div>
                    `;
                    
                    elements.mappingsContainer.appendChild(newRow);
                }
                
                // Add mappings from template
                mappings.forEach((mapping, index) => {
                    // For the first mapping, update the existing row
                    if (index === 0) {
                        const firstRow = elements.mappingsContainer.querySelector('.mapping-row');
                        const excelField = firstRow.querySelector('.excel-field');
                        const formField = firstRow.querySelector('.form-field');
                        const recurringCheck = firstRow.querySelector('.recurring-check');
                        
                        // Update the dropdowns with current data
                        updateExcelFieldOptions(excelColumns);
                        updateDatabaseFieldOptions();
                        
                        // Set values
                        const excelColumn = mapping.excelColumn || mapping.ExcelColumn || '';
                        const databaseField = mapping.databaseField || mapping.DatabaseField || '';
                        const isRecurring = mapping.isRecurring || mapping.IsRecurring || false;
                        
                        console.log(`Setting mapping ${index}: Excel=${excelColumn}, DB=${databaseField}, Recurring=${isRecurring}`);
                        
                        // Set Excel field selection
                        for (let i = 0; i < excelField.options.length; i++) {
                            if (excelField.options[i].value === excelColumn) {
                                excelField.selectedIndex = i;
                                break;
                            }
                        }
                        
                        // Set database field selection
                        for (let i = 0; i < formField.options.length; i++) {
                            if (formField.options[i].value === databaseField) {
                                formField.selectedIndex = i;
                                break;
                            }
                        }
                        
                        recurringCheck.checked = isRecurring;
                    } else {
                        // Add new row for additional mappings
                        addMappingRow();
                        
                        // Get the newly added row
                        const rows = elements.mappingsContainer.querySelectorAll('.mapping-row');
                        const lastRow = rows[rows.length - 1];
                        
                        const excelField = lastRow.querySelector('.excel-field');
                        const formField = lastRow.querySelector('.form-field');
                        const recurringCheck = lastRow.querySelector('.recurring-check');
                        
                        // Set values
                        const excelColumn = mapping.excelColumn || mapping.ExcelColumn || '';
                        const databaseField = mapping.databaseField || mapping.DatabaseField || '';
                        const isRecurring = mapping.isRecurring || mapping.IsRecurring || false;
                        
                        console.log(`Setting mapping ${index}: Excel=${excelColumn}, DB=${databaseField}, Recurring=${isRecurring}`);
                        
                        // Update the dropdowns for the new row
                        updateExcelFieldOptionsForDropdown(excelField, excelColumns);
                        updateDatabaseFieldOptionsForDropdown(formField);
                        
                        // Set Excel field selection
                        for (let i = 0; i < excelField.options.length; i++) {
                            if (excelField.options[i].value === excelColumn) {
                                excelField.selectedIndex = i;
                                break;
                            }
                        }
                        
                        // Set database field selection
                        for (let i = 0; i < formField.options.length; i++) {
                            if (formField.options[i].value === databaseField) {
                                formField.selectedIndex = i;
                                break;
                            }
                        }
                        
                        recurringCheck.checked = isRecurring;
                    }
                });
                
                // Update submit button text
                elements.submitBtn.textContent = 'Update Template';
                
                // Add a hidden input for template ID
                let templateIdInput = document.getElementById('templateId');
                if (!templateIdInput) {
                    templateIdInput = document.createElement('input');
                    templateIdInput.type = 'hidden';
                    templateIdInput.id = 'templateId';
                    templateIdInput.name = 'templateId';
                    elements.packingForm.appendChild(templateIdInput);
                }
                templateIdInput.value = id;
                
                showAlert('Template loaded for editing', 'success');
            } else {
                showAlert(`Error loading template: ${result.message}`, 'danger');
            }
        })
        .catch(error => {
            console.error('Error loading template for editing:', error);
            showAlert('Error loading template for editing. Please try again.', 'danger');
        });
};

function showAlert(message, type = 'info') {
    const alertContainer = document.querySelector('.alert-container');
    
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show fade-in`;
    alertDiv.setAttribute('role', 'alert');
    
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    alertContainer.appendChild(alertDiv);
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.classList.remove('show');
            setTimeout(() => {
                if (alertDiv.parentNode) {
                    alertDiv.parentNode.removeChild(alertDiv);
                }
            }, 150);
        }
    }, 5000);
}







// Function to download from template settings
// async function downloadFromTemplateSettings(templateId) {
//     // Check if we have a packing list ID
//     const packingListId = getPackingListId();
//     if (!packingListId) {
//         showToast('No packing list available. Please create a packing list first.', 'warning');
//         return;
//     }
    
//     // Download the Excel with template data
//     await downloadExcelWithTemplateData(templateId);
// }

// // Update the download button click handler in template management page
// function setupDownloadButtons() {
//     // Find all download buttons and attach event listeners
//     const downloadButtons = document.querySelectorAll('.download-template-btn');
//     downloadButtons.forEach(button => {
//         button.addEventListener('click', async function() {
//             const templateId = this.getAttribute('data-template-id');
//             await downloadFromTemplateSettings(templateId);
//         });
//     });
// }

// // // Initialize download buttons when DOM is ready
// // document.addEventListener('DOMContentLoaded', function() {
// //     setupDownloadButtons();
// // });

// // Function to handle download from the packing list page
// async function downloadFromPackingListPage(templateId) {
//     // Check if we have a packing list ID
//     const packingListId = getPackingListId();
//     if (!packingListId) {
//         showToast('No packing list available. Please create a packing list first.', 'warning');
//         return;
//     }
    
//     // Download the Excel with template data
//     await downloadExcelWithTemplateData(templateId);
// }

// // Function to open template settings with packing list ID
// function openTemplateSettings() {
//     const packingListId = getPackingListId();
//     if (!packingListId) {
//         showToast('No packing list available. Please create a packing list first.', 'warning');
//         return;
//     }
    
//     // Open template settings with packing list ID as parameter
//     window.open(`../../TemplateSettings/CreateTemplate.html?packingListId=${packingListId}`, '_blank');
// }

// // Update the template settings button click handler
// document.getElementById('openModalTemplate').addEventListener('click', function() {
//     openTemplateSettings();
// });

// Function to get packing list ID from URL parameters
function getPackingListIdFromUrl() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('packingListId');
}

// Initialize packing list ID from URL parameters if available
document.addEventListener('DOMContentLoaded', function() {
    const packingListIdFromUrl = getPackingListIdFromUrl();
    if (packingListIdFromUrl) {
        savePackingListId(packingListIdFromUrl);
    }
});



// Function to download Excel with template data
// Function to download Excel with template data
// Function to download Excel with template data
async function downloadExcelWithTemplateData(templateId) {
    try {
        // Get the packing list ID from localStorage
        const packingListId = getPackingListId();
        if (!packingListId) {
            showToast('No packing list selected. Please create a packing list first.', 'warning');
            return;
        }

        showToast('Preparing your download...', 'info');

        // Fetch the Excel file with the packing list ID as a query parameter
        const response = await fetch(`/api/template/${templateId}/download-with-data?packingListId=${packingListId}`);
        
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Failed to download template');
        }
        
        // Get the filename from the response headers
        const contentDisposition = response.headers.get('content-disposition');
        let filename = 'template.xlsx';
        if (contentDisposition) {
            const filenameMatch = contentDisposition.match(/filename="(.+)"/);
            if (filenameMatch && filenameMatch.length === 2) {
                filename = filenameMatch[1];
            }
        }
        
        // Convert the response to a blob
        const blob = await response.blob();
        
        // Create a download link
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(url);
        
        showToast('Download completed successfully!', 'success');
    } catch (error) {
        console.error('Error downloading template:', error);
        showToast(`Error: ${error.message}`, 'error');
    }
}









// Function to save packing list ID to localStorage
function savePackingListId(id) {
    localStorage.setItem('currentPackingListId', id);
}

// Function to get packing list ID from localStorage
function getPackingListId() {
    return localStorage.getItem('currentPackingListId');
}

// Function to clear packing list ID from localStorage
function clearPackingListId() {
    localStorage.removeItem('currentPackingListId');
}

// Check if packing list ID exists in localStorage
function hasPackingListId() {
    return !!getPackingListId();
}

// Function to show toast notifications
function showToast(message, type = 'info') {
    const alertContainer = document.querySelector('.alert-container');
    
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show fade-in`;
    alertDiv.setAttribute('role', 'alert');
    
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    alertContainer.appendChild(alertDiv);
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.classList.remove('show');
            setTimeout(() => {
                if (alertDiv.parentNode) {
                    alertDiv.parentNode.removeChild(alertDiv);
                }
            }, 150);
        }
    }, 5000);
}