
        // Global variables
        let packingListData = null;
        let activeTemplate = null;
        let packingListId = null;
        let allTemplates = [];

        // Get URL parameters
        function getUrlParameter(name) {
            name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
            const regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
            const results = regex.exec(location.search);
            return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
        }

        // Initialize page
        $(document).ready(function() {
            // Get packing list ID from URL
            packingListId = getUrlParameter('id');
            if (!packingListId) {
                showToast('No packing list ID provided', 'error');
                return;
            }
            
            // Load packing list data
            loadPackingList(packingListId);
            
            // Load all templates and find the active one
            loadAllTemplates();
        });

        // Load packing list data
        async function loadPackingList(id) {
            try {
                showToast('Loading packing list...', 'info');
                
                const response = await fetch(`/api/packaging-list/${id}`);
                if (!response.ok) throw new Error('Failed to load packing list');
                
                const data = await response.json();
                packingListData = data.data;
                
                // Display packing details
                displayPackingDetails(packingListData.packingDetails);
                
                showToast('Packing list loaded successfully', 'success');
            } catch (error) {
                console.error('Error loading packing list:', error);
                showToast('Error loading packing list', 'error');
            }
        }

        // Load all templates and find the active one
        async function loadAllTemplates() {
            try {
                const response = await fetch('/api/template');
                if (!response.ok) throw new Error('Failed to load templates');
                
                const data = await response.json();
                if (data.success) {
                    allTemplates = data.data;
                    
                    // Find the active template
                    activeTemplate = allTemplates.find(template => 
                        template.isActive === true || template.IsActive === true
                    );
                    
                    if (activeTemplate) {
                        console.log('Active template found:', activeTemplate);
                        
                        // If packing details are already loaded, refresh with template
                        if (packingListData && packingListData.packingDetails) {
                            displayPackingDetails(packingListData.packingDetails);
                        }
                    } else {
                        console.log('No active template found');
                        showToast('No active template found. Showing all available fields.', 'warning');
                    }
                } else {
                    console.error('Failed to load templates:', data.message);
                    showToast('Error loading templates', 'error');
                }
            } catch (error) {
                console.error('Error loading templates:', error);
                showToast('Error loading templates', 'error');
            }
        }

        // Display packing details using div-based grid
        function displayPackingDetails(details) {
            const container = $('#excelGridContainer');
            const tbody = $('#packingDetailsTableBody');
            const thead = $('#tableHeaders');
            
            // Clear existing content
            container.empty();
            tbody.empty();
            thead.empty();
            
            if (!details || details.length === 0) {
                container.html(`
                    <div class="empty-state">
                        <i class="fas fa-inbox"></i>
                        <h3>No packing details found</h3>
                    </div>
                `);
                return;
            }
            
            let headers = [];
            let rowsData = [];
            
            // If we have an active template, use its mappings
            if (activeTemplate && activeTemplate.mappings && activeTemplate.mappings.length > 0) {
                // Get headers from template mappings
                headers = activeTemplate.mappings.map(mapping => mapping.excelColumn);
                
                // Populate hidden table for PDF/Excel export
                const headerRow = $('<tr>');
                activeTemplate.mappings.forEach(mapping => {
                    headerRow.append(`<th>${mapping.excelColumn}</th>`);
                });
                thead.append(headerRow);
                
                // Create rows for each packing detail
                details.forEach(detail => {
                    const rowData = [];
                    const hiddenRow = $('<tr>');
                    
                    activeTemplate.mappings.forEach(mapping => {
                        // Convert database field to camelCase for JavaScript property access
                        const camelCaseField = mapping.databaseField.charAt(0).toLowerCase() + 
                                              mapping.databaseField.slice(1);
                        const value = detail[camelCaseField] || '';
                        rowData.push(value);
                        hiddenRow.append(`<td>${value}</td>`);
                    });
                    
                    rowsData.push(rowData);
                    tbody.append(hiddenRow);
                });
            } else {
                // Fallback: show all available fields
                const sampleDetail = details[0];
                
                // Get headers from all available fields
                Object.keys(sampleDetail).forEach(key => {
                    // Skip internal fields like 'id'
                    if (key !== 'id' && key !== 'packingListId') {
                        headers.push(key);
                    }
                });
                
                // Populate hidden table
                const headerRow = $('<tr>');
                headers.forEach(header => {
                    headerRow.append(`<th>${header}</th>`);
                });
                thead.append(headerRow);
                
                // Create rows for each packing detail
                details.forEach(detail => {
                    const rowData = [];
                    const hiddenRow = $('<tr>');
                    
                    Object.keys(detail).forEach(key => {
                        // Skip internal fields
                        if (key !== 'id' && key !== 'packingListId') {
                            const value = detail[key] || '';
                            rowData.push(value);
                            hiddenRow.append(`<td>${value}</td>`);
                        }
                    });
                    
                    rowsData.push(rowData);
                    tbody.append(hiddenRow);
                });
            }
            
            // Render the visual grid
            renderExcelGrid(headers, rowsData);
        }

        // Render Excel Grid with divs
        function renderExcelGrid(headers, rows) {
            const container = document.getElementById('excelGridContainer');
            
            let html = '<div class="excel-grid">';
            
            // Header Row
            html += '<div class="excel-row header-row">';
            headers.forEach(header => {
                html += `<div class="excel-cell header-cell">${header}</div>`;
            });
            html += '</div>';
            
            // Data Rows
            rows.forEach(row => {
                html += '<div class="excel-row data-row">';
                row.forEach(cell => {
                    html += `<div class="excel-cell data-cell">${cell}</div>`;
                });
                html += '</div>';
            });
            
            html += '</div>';
            container.innerHTML = html;
        }

        // Download Excel with template data
// Download Excel with template data (frontend only)
function downloadExcel() {
    if (!activeTemplate && !packingListData) {
        showToast('No data available for download', 'warning');
        return;
    }
    
    try {
        showToast('Preparing your Excel download...', 'info');
        
        // Create a new workbook
        const wb = XLSX.utils.book_new();
        
        // Prepare data for the main sheet
        let headers = [];
        let rowsData = [];
        
        // If we have an active template, use its mappings
        if (activeTemplate && activeTemplate.mappings && activeTemplate.mappings.length > 0) {
            // Get headers from template mappings
            headers = activeTemplate.mappings.map(mapping => mapping.excelColumn);
            
            // Create rows for each packing detail
            packingListData.packingDetails.forEach(detail => {
                const rowData = [];
                
                activeTemplate.mappings.forEach(mapping => {
                    // Convert database field to camelCase for JavaScript property access
                    const camelCaseField = mapping.databaseField.charAt(0).toLowerCase() + 
                                          mapping.databaseField.slice(1);
                    const value = detail[camelCaseField] || '';
                    rowData.push(value);
                });
                
                rowsData.push(rowData);
            });
        } else {
            // Fallback: show all available fields
            const sampleDetail = packingListData.packingDetails[0];
            
            // Get headers from all available fields
            Object.keys(sampleDetail).forEach(key => {
                // Skip internal fields like 'id'
                if (key !== 'id' && key !== 'packingListId') {
                    headers.push(key);
                }
            });
            
            // Create rows for each packing detail
            packingListData.packingDetails.forEach(detail => {
                const rowData = [];
                
                Object.keys(detail).forEach(key => {
                    // Skip internal fields
                    if (key !== 'id' && key !== 'packingListId') {
                        const value = detail[key] || '';
                        rowData.push(value);
                    }
                });
                
                rowsData.push(rowData);
            });
        }
        
        // Create worksheet with packing details
        const wsData = [headers, ...rowsData];
        const ws = XLSX.utils.aoa_to_sheet(wsData);
        
        // Add the worksheet to the workbook
        XLSX.utils.book_append_sheet(wb, ws, "Packing Details");
        
        // Create a second worksheet with packing list information
        const infoData = [
            ["Packing List Information"],
            ["Company", packingListData.company || ""],
            ["Buyer", packingListData.buyer || ""],
            ["Style", packingListData.style || ""],
            ["Purchase Order", packingListData.purchaseOrder || ""],
            ["Status", packingListData.status || ""],
            ["Created Date", new Date(packingListData.createdAt).toLocaleDateString()],
            ["Country of Origin", packingListData.countryOfOrigin || ""],
            ["Port of Loading", packingListData.portOfLoading || ""],
            ["Country of Destination", packingListData.countryOfDestination || ""],
            ["Port of Discharge", packingListData.portDischarge || ""]
        ];
        
        const infoWs = XLSX.utils.aoa_to_sheet(infoData);
        XLSX.utils.book_append_sheet(wb, infoWs, "Information");
        
        // Generate filename
        const companyName = packingListData.company || "PackingList";
        const poNumber = packingListData.purchaseOrder || "";
        const dateStr = new Date().toISOString().slice(0, 10);
        const filename = `${companyName}_${poNumber}_${dateStr}.xlsx`;
        
        // Save the file
        XLSX.writeFile(wb, filename);
        
        showToast('Excel download completed successfully!', 'success');
    } catch (error) {
        console.error('Error generating Excel:', error);
        showToast(`Error: ${error.message}`, 'error');
    }
}

// Download PDF with template data (frontend only)
function downloadPDF() {
    if (!activeTemplate && !packingListData) {
        showToast('No data available for download', 'warning');
        return;
    }
    
    try {
        showToast('Preparing your PDF download...', 'info');
        
        // Prepare data for the table
        let headers = [];
        let data = [];
        
        // If we have an active template, use its mappings
        if (activeTemplate && activeTemplate.mappings && activeTemplate.mappings.length > 0) {
            // Get headers from template mappings
            headers = activeTemplate.mappings.map(mapping => mapping.excelColumn);
            
            // Create rows for each packing detail
            packingListData.packingDetails.forEach(detail => {
                const rowData = [];
                
                activeTemplate.mappings.forEach(mapping => {
                    // Convert database field to camelCase for JavaScript property access
                    const camelCaseField = mapping.databaseField.charAt(0).toLowerCase() + 
                                          mapping.databaseField.slice(1);
                    const value = detail[camelCaseField] || '';
                    rowData.push(value);
                });
                
                data.push(rowData);
            });
        } else {
            // Fallback: show all available fields
            const sampleDetail = packingListData.packingDetails[0];
            
            // Get headers from all available fields
            Object.keys(sampleDetail).forEach(key => {
                // Skip internal fields like 'id'
                if (key !== 'id' && key !== 'packingListId') {
                    headers.push(key);
                }
            });
            
            // Create rows for each packing detail
            packingListData.packingDetails.forEach(detail => {
                const rowData = [];
                
                Object.keys(detail).forEach(key => {
                    // Skip internal fields
                    if (key !== 'id' && key !== 'packingListId') {
                        const value = detail[key] || '';
                        rowData.push(value);
                    }
                });
                
                data.push(rowData);
            });
        }
        
        // Create PDF
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();
        
        // Add title
        doc.setFontSize(18);
        doc.text('Packing List Details', 14, 15);
        
        // Add packing list info
        doc.setFontSize(12);
        doc.text(`Company: ${packingListData.company || ''}`, 14, 25);
        doc.text(`Buyer: ${packingListData.buyer || ''}`, 14, 32);
        doc.text(`PO: ${packingListData.purchaseOrder || ''}`, 14, 39);
        doc.text(`Date: ${new Date(packingListData.createdAt).toLocaleDateString()}`, 14, 46);
        
        // Add table
        doc.autoTable({
            head: [headers],
            body: data,
            startY: 55,
            styles: {
                fontSize: 10,
                cellPadding: 3,
            },
            headStyles: {
                fillColor: [242, 242, 242],
                textColor: [0, 0, 0],
                fontStyle: 'bold',
            },
            alternateRowStyles: {
                fillColor: [249, 249, 249],
            },
        });
        
        // Generate filename
        const companyName = packingListData.company || "PackingList";
        const poNumber = packingListData.purchaseOrder || "";
        const dateStr = new Date().toISOString().slice(0, 10);
        const filename = `${companyName}_${poNumber}_${dateStr}.pdf`;
        
        // Save the PDF
        doc.save(filename);
        
        showToast('PDF download completed successfully!', 'success');
    } catch (error) {
        console.error('Error generating PDF:', error);
        showToast(`Error: ${error.message}`, 'error');
    }
}
        // Navigate back to the list page
        function goBack() {
            window.location.href = '../Packing/PackiListpage/packingList.html';  
        }

        // Edit the packing list
        function editPackingList() {
            window.location.href = `../PackingListCreation/packingcreation.html?id=${packingListId}`;
        }

        // Show toast notification
        function showToast(message, type = 'info') {
            const toastContainer = document.querySelector('.toast-container');
            const toastId = 'toast-' + Date.now();
            
            const toastHTML = `
                <div id="${toastId}" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="toast-header">
                        <strong class="me-auto">${type.charAt(0).toUpperCase() + type.slice(1)}</strong>
                        <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                    <div class="toast-body">
                        ${message}
                    </div>
                </div>
            `;
            
            toastContainer.insertAdjacentHTML('beforeend', toastHTML);
            
            const toastElement = document.getElementById(toastId);
            const toast = new bootstrap.Toast(toastElement, {
                autohide: true,
                delay: 3000
            });
            
            toast.show();
            
            // Remove toast element after it's hidden
            toastElement.addEventListener('hidden.bs.toast', function () {
                toastElement.remove();
            });
        }
 