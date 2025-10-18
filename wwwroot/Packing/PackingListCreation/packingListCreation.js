



let packagingRowCounter = 1;
let packagingListId = null;
let declarationQuestions = [];

// Get URL parameters to check if we're editing an existing record
function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    const regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    const results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

// Save packing list ID to localStorage
function savePackingListId(id) {
    localStorage.setItem('currentPackingListId', id);
}

// Get packing list ID from localStorage
function getPackingListId() {
    return localStorage.getItem('currentPackingListId');
}

// Clear packing list ID from localStorage
function clearPackingListId() {
    localStorage.removeItem('currentPackingListId');
}

// Check if packing list ID exists in localStorage
function hasPackingListId() {
    return !!getPackingListId();
}

// Load declaration questions
async function loadDeclarationQuestions() {
    try {
        const response = await fetch('/api/packaging-list/declaration-questions');
        if (!response.ok) throw new Error('Failed to load declaration questions');
        
        const result = await response.json();
        console.log("API Response:", result);
        
        // Extract the questions array from the response object
        let questions = [];
        
        if (result.data && Array.isArray(result.data)) {
            // If the API returns an array of objects with questionNumber and questionText
            questions = result.data;
        } else if (result.data && Array.isArray(result.data.questions)) {
            // If the API returns an object with a questions property
            questions = result.data.questions;
        } else if (Array.isArray(result)) {
            // If the API returns an array directly
            questions = result;
        }
        
        // If we have an array of strings, convert to objects
        if (questions.length > 0 && typeof questions[0] === 'string') {
            questions = questions.map((text, index) => ({
                questionNumber: index + 1,
                questionText: text
            }));
        }
        
        console.log("Processed questions:", questions);
        
        // Store the questions globally
        declarationQuestions = questions;
        
        renderDeclarationQuestions(questions);
    } catch (error) {
        console.error('Error loading declaration questions:', error);
        showToast('Error loading declaration questions', 'error');
        
        // Fallback to default questions
        const defaultQuestions = [
            { questionNumber: 1, questionText: "Have unacceptable packaging materials or bamboo products been used as packaging or dunnage in the consignment covered by this document?" },
            { questionNumber: 2, questionText: "Has solid timber packaging/dunnage been used in consignments covered by this document?" },
            { questionNumber: 3, questionText: "All timber packaging/dunnage used in the consignment has been (Please indicate below) Treated in compliance with Department of Agriculture and Water Resources treatment requirements" },
            { questionNumber: 4, questionText: "statement to be removed from document when not relevant" }
        ];
        declarationQuestions = defaultQuestions;
        renderDeclarationQuestions(defaultQuestions);
    }
}

// Render declaration questions
function renderDeclarationQuestions(questions) {
    const container = document.getElementById('declarationContainer');
    let html = '<table class="declaration-table">';
    
    // Render the dynamic questions from the API
    questions.forEach((question, index) => {
        const isDateField = question.questionText.toLowerCase().includes('date');
        const inputType = isDateField ? 'date' : 'text';
        
        html += `
            <tr>
                <td>${question.questionNumber}</td>
                <td>${question.questionText}</td>
                <td><input type="${inputType}" class="declaration-answer" data-question="${question.questionNumber}" style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px;"></td>
            </tr>
        `;
    });
    
    // Add an extra row with a static date question
    const dateQuestionNumber = questions.length + 1;
    html += `
        <tr>
            <td>${dateQuestionNumber}</td>
            <td>Issued Date</td>
            <td><input type="date" class="declaration-answer" data-question="${dateQuestionNumber}" style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px;"></td>
        </tr>
    `;
    
    html += '</table>';
    container.innerHTML = html;
}

// Load packaging list data if editing
async function loadPackagingList(id) {
    console.log("loadde caleed",id)
    try {
        showToast('Loading packaging list...', 'info');
        
        const response = await fetch(`/api/packaging-list/${id}`);
        if (!response.ok) throw new Error('Failed to load packaging list');
        
        const data = await response.json();
        const packagingList = data.data;
        console.log("data to update",packagingList)
        // Set form fields
        $('#company').val(packagingList.company);
        $('#buyer').val(packagingList.buyer);
        $('#style').val(packagingList.style);
        $('#po').val(packagingList.purchaseOrder);
        $('#styleBuyer').val(packagingList.styleBuyer);
        $('#shippedBy').val(packagingList.shippedBy);
        $('#shortDesc').val(packagingList.shortDescription);
        $('#size').val(packagingList.size);
        $('#color').val(packagingList.color);
        $('#brand').val(packagingList.brand);
        $('#packing').val(packagingList.packing);
        $('#pcsPerSet').val(packagingList.pcsPerSet);
        $('#productNo').val(packagingList.productNo);
        $('#countryOrigin').val(packagingList.countryOfOrigin);
        $('#portLoading').val(packagingList.portOfLoading);
        $('#countryDestination').val(packagingList.countryOfDestination);
        $('#portDischarge').val(packagingList.portOfDischarge);
        $('#incoterm').val(packagingList.incoterm);
        $('#ormsNo').val(packagingList.ormsNo);
        $('#lmkgNo').val(packagingList.lmkgNo);
        $('#ormsStyleNo').val(packagingList.ormsStyleNo);
        $('#status').val(packagingList.status);
        
        // Buyer specific fields
        $('#itemNo').val(packagingList.itemNo);
        $('#pod').val(packagingList.pod);
        $('#boi').val(packagingList.boi);
        $('#wwwk').val(packagingList.wwwk);
        $('#noOfColor').val(packagingList.noOfColor);
        $('#keyCode').val(packagingList.keyCode);
        $('#supplierCode').val(packagingList.supplierCode);
        
        // Additional fields
        $('#cartons').val(packagingList.cartons);
        $('#orderQtyPac').val(packagingList.orderQtyPac);
        $('#orderQtyPcs').val(packagingList.orderQtyPcs);
        $('#shipQtyPac').val(packagingList.shipQtyPac);
        $('#shipQtyPcs').val(packagingList.shipQtyPcs);
        $('#destination').val(packagingList.destination);
        
        // Load packing details
        loadPackingDetails(packagingList.packingDetails);
        
        // Load declaration answers
        loadDeclarationAnswers(packagingList.declarationAnswers);
        
        // Update UI
        packagingListId = id;
        // Save ID to localStorage
        savePackingListId(id);
        document.getElementById('pageTitle').textContent = 'Edit Packing List';
        document.getElementById('saveButtonText').textContent = 'UPDATE';
        console.log("statys",packagingList.status)
        if (packagingList.status === 'locked') {
             document.querySelectorAll('input, select, textarea, button').forEach(el => {
    // Skip the one you want to remain enabled
                if (!el.classList.contains('btn-cancel')) { 
                el.disabled = true;
                }
            });
        }
        showToast('Packaging list loaded successfully', 'success');
    } catch (error) {
        console.error('Error loading packaging list:', error);
        showToast('Error loading packaging list', 'error');
    }
}

// Load packing details
function loadPackingDetails(details) {
    if (!details || details.length === 0) return;
    
    const tbody = document.getElementById('packingTableBody');
    tbody.innerHTML = '';
    packagingRowCounter = 0;
    
    details.forEach(detail => {
        packagingRowCounter++;
        const row = `
            <tr>
                <td>${detail.sl}</td>
                <td><input type="text" class="noOfCarton" value="${detail.noOfCarton}"></td>
                <td><input type="text" class="start" value="${detail.start}"></td>
                <td><input type="text" class="end" value="${detail.end}"></td>
                <td><input type="text" class="sizeName" value="${detail.sizeName}"></td>
                <td><input type="text" class="ratio" value="${detail.ratio}"></td>
                <td><input type="text" class="articleNo" value="${detail.articleNo}"></td>
                <td><input type="text" class="pcsPack" value="${detail.pcsPack}"></td>
                <td><input type="text" class="pacCarton" value="${detail.pacCarton}"></td>
                <td><input type="text" class="orderQty" value="${detail.orderQty}"></td>
                <td><input type="text" class="totalPcs" value="${detail.totalPcs}"></td>
                <td><input type="text" class="totalPacs" value="${detail.totalPacs}"></td>
                <td><input type="text" class="gWt" value="${detail.gWt}"></td>
                <td><input type="text" class="nWt" value="${detail.nWt}"></td>
                <td><input type="text" class="totalWt" value="${detail.totalWt}"></td>
                <td><input type="text" class="l" value="${detail.l}"></td>
                <td><input type="text" class="w" value="${detail.w}"></td>
                <td><input type="text" class="h" value="${detail.h}"></td>
                <td><input type="text" class="cbm" value="${detail.cbm}"></td>
            </tr>
        `;
        tbody.innerHTML += row;
    });
    
    updateSummary();
}

// Load declaration answers
function loadDeclarationAnswers(answers) {
    if (!answers || answers.length === 0) return;
    
    answers.forEach(answer => {
        const input = document.querySelector(`.declaration-answer[data-question="${answer.questionNumber}"]`);
        if (input) {
            if (answer.answerDate) {
                input.value = answer.answerDate.split('T')[0]; // Format date for input
            } else {
                input.value = answer.answer;
            }
        }
    });
}

function addPackingRow() {
    packagingRowCounter++;
    const newRow = `
        <tr>
            <td>${packagingRowCounter}</td>
            <td><input type="text" class="noOfCarton"></td>
            <td><input type="text" class="start"></td>
            <td><input type="text" class="end"></td>
            <td><input type="text" class="sizeName"></td>
            <td><input type="text" class="ratio"></td>
            <td><input type="text" class="articleNo"></td>
            <td><input type="text" class="pcsPack"></td>
            <td><input type="text" class="pacCarton"></td>
            <td><input type="text" class="orderQty"></td>
            <td><input type="text" class="totalPcs"></td>
            <td><input type="text" class="totalPacs"></td>
            <td><input type="text" class="gWt"></td>
            <td><input type="text" class="nWt"></td>
            <td><input type="text" class="totalWt"></td>
            <td><input type="text" class="l"></td>
            <td><input type="text" class="w"></td>
            <td><input type="text" class="h"></td>
            <td><input type="text" class="cbm"></td>
        </tr>
    `;
    $('#packingTableBody').append(newRow);
    updateSummary();
}

function removePackingRow() {
    if (packagingRowCounter > 1) {
        $('#packingTableBody tr:last').remove();
        packagingRowCounter--;
        updateSummary();
    } else {
        showToast('Cannot remove the last row!', 'error');
    }
}

function updateSummary() {
    const rows = $('#packingTableBody tr');
    $('#summaryTableBody').empty();
    
    rows.each(function(index) {
        const sizeName = $(this).find('.sizeName').val() || '-';
        const orderQty = $(this).find('.orderQty').val() || '-';
        const totalPcs = $(this).find('.totalPcs').val() || '-';
        
        let variance = '-';
        let percentage = '-';
        
        if (orderQty !== '-' && totalPcs !== '-' && !isNaN(orderQty) && !isNaN(totalPcs)) {
            variance = parseFloat(totalPcs) - parseFloat(orderQty);
            percentage = ((variance / parseFloat(orderQty)) * 100).toFixed(2) + '%';
            variance = variance.toFixed(2);
        }
        
        const summaryRow = `
            <tr>
                <td>${index + 1}</td>
                <td>${sizeName}</td>
                <td>${orderQty}</td>
                <td>${totalPcs}</td>
                <td>${variance}</td>
                <td>${percentage}</td>
            </tr>
        `;
        $('#summaryTableBody').append(summaryRow);
    });
}

 $(document).on('input', '.orderQty, .totalPcs, .sizeName', function() {
    updateSummary();
});

 $(document).on('input', '.pcsPack, .pacCarton', function() {
    const row = $(this).closest('tr');
    const pcsPack = parseFloat(row.find('.pcsPack').val()) || 0;
    const pacCarton = parseFloat(row.find('.pacCarton').val()) || 0;
    const totalPcs = pcsPack * pacCarton;
    row.find('.totalPcs').val(totalPcs || '');
    updateSummary();
});

async function saveForm() {
    // Validate required fields
    if (!$('#company').val().trim()) {
        showToast('Company is required', 'error');
        return;
    }
    if (!$('#buyer').val().trim()) {
        showToast('Buyer is required', 'error');
        return;
    }

    // Collect all form data
    const formData = {
        company: $('#company').val(),
        buyer: $('#buyer').val(),
        style: $('#style').val(),
        purchaseOrder: $('#po').val(),
        styleBuyer: $('#styleBuyer').val(),
        shippedBy: $('#shippedBy').val(),
        shortDescription: $('#shortDesc').val(),
        size: $('#size').val(),
        color: $('#color').val(),
        brand: $('#brand').val(),
        packing: $('#packing').val(),
        pcsPerSet: $('#pcsPerSet').val(),
        productNo: $('#productNo').val(),
        countryOfOrigin: $('#countryOrigin').val(),
        portOfLoading: $('#portLoading').val(),
        countryOfDestination: $('#countryDestination').val(),
        portOfDischarge: $('#portDischarge').val(),
        incoterm: $('#incoterm').val(),
        ormsNo: $('#ormsNo').val(),
        lmkgNo: $('#lmkgNo').val(),
        ormsStyleNo: $('#ormsStyleNo').val(),
        status: $('#status').val(),
        
        // Buyer Specific Fields
        itemNo: $('#itemNo').val(),
        pod: $('#pod').val(),
        boi: $('#boi').val(),
        wwwk: $('#wwwk').val(),
        noOfColor: $('#noOfColor').val(),
        keyCode: $('#keyCode').val(),
        supplierCode: $('#supplierCode').val(),
        
        // Additional Fields
        cartons: $('#cartons').val(),
        orderQtyPac: $('#orderQtyPac').val(),
        orderQtyPcs: $('#orderQtyPcs').val(),
        shipQtyPac: $('#shipQtyPac').val(),
        shipQtyPcs: $('#shipQtyPcs').val(),
        destination: $('#destination').val(),
        
        packingDetails: [],
        declarationAnswers: []
    };

    // Collect packing details
    $('#packingTableBody tr').each(function(index) {
        const row = {
            sl: index + 1,
            noOfCarton: $(this).find('.noOfCarton').val() || "",
            start: $(this).find('.start').val() || "",
            end: $(this).find('.end').val() || "",
            sizeName: $(this).find('.sizeName').val() || "",
            ratio: $(this).find('.ratio').val() || "",
            articleNo: $(this).find('.articleNo').val() || "",
            pcsPack: $(this).find('.pcsPack').val() || "",
            pacCarton: $(this).find('.pacCarton').val() || "",
            orderQty: $(this).find('.orderQty').val() || "",
            totalPcs: $(this).find('.totalPcs').val() || "",
            totalPacs: $(this).find('.totalPacs').val() || "",
            gWt: $(this).find('.gWt').val() || "",
            nWt: $(this).find('.nWt').val() || "",
            totalWt: $(this).find('.totalWt').val() || "",
            l: $(this).find('.l').val() || "",
            w: $(this).find('.w').val() || "",
            h: $(this).find('.h').val() || "",
            cbm: $(this).find('.cbm').val() || ""
        };
        formData.packingDetails.push(row);
    });

    // Validate packing details
    if (formData.packingDetails.length === 0) {
        showToast('At least one packing detail is required', 'error');
        return;
    }

    // Collect declaration answers
    $('.declaration-answer').each(function() {
        const questionNumber = parseInt($(this).data('question'));
        let question = null;
        
        // Find the question in the declarationQuestions array
        if (declarationQuestions && declarationQuestions.length > 0) {
            question = declarationQuestions.find(q => q.questionNumber === questionNumber);
        }
        
        // If we can't find the question, create a default one
        if (!question) {
            question = {
                questionNumber: questionNumber,
                questionText: `Question ${questionNumber}`
            };
        }
        
        const answer = $(this).val();
        let answerDate = null;
        
        if ($(this).attr('type') === 'date' && answer) {
            answerDate = new Date(answer).toISOString();
        }
        
        formData.declarationAnswers.push({
            questionNumber: questionNumber,
            question: question.questionText,
            answer: answer,
            answerDate: answerDate
        });
    });

    // Validate declaration answers
    if (formData.declarationAnswers.length === 0) {
        showToast('At least one declaration answer is required', 'error');
        return;
    }

    try {
        showToast('Saving...', 'info');
        
        const url = packagingListId 
            ? `/api/packaging-list/${packagingListId}` 
            : '/api/packaging-list';
        const method = packagingListId ? 'PUT' : 'POST';
        
        console.log('Sending data to server:', formData);
        
        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(formData),
        });
        
        if (!response.ok) {
            const errorData = await response.json();
            console.error('Server response:', errorData);
            
            let errorMessage = 'Failed to save packaging list';
            if (errorData.errors && errorData.errors.length > 0) {
                errorMessage = errorData.errors.join(', ');
            } else if (errorData.message) {
                errorMessage = errorData.message;
            } else if (errorData.title) {
                errorMessage = errorData.title;
            }
            
            throw new Error(errorMessage);
        }
        
        const result = await response.json();
        
        if (!packagingListId) {
            packagingListId = result.data.id;
            // Save ID to localStorage
            savePackingListId(packagingListId);
            document.getElementById('pageTitle').textContent = 'Edit Packing List';
            document.getElementById('saveButtonText').textContent = 'UPDATE';
        }
        
        showToast('Packaging list saved successfully!', 'success');
        console.log('Saved data:', result.data);
    } catch (error) {
        console.error('Error saving form:', error);
        showToast(`Error: ${error.message}`, 'error');
    }
}

function cancelForm() {
    if (confirm('Are you sure you want to cancel? All unsaved data will be lost.')) {
        // Clear the packing list ID from localStorage
        clearPackingListId();
        window.location.href = '../PackiListpage/packingList.html'; // Redirect to list page
    }
}

function showToast(message, type = 'info') {
    const toast = document.getElementById('toast');
    toast.textContent = message;
    toast.className = 'toast show ' + type;
    
    setTimeout(() => {
        toast.classList.remove('show');
    }, 3000);
}

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

        // Fetch template details
        const templateResponse = await fetch(`/api/template/${templateId}`);
        const templateData = await templateResponse.json();
        
        if (!templateData.success) {
            throw new Error(templateData.message || 'Failed to fetch template');
        }
        
        const template = templateData.data;
        const mappings = template.mappings || [];

        // Fetch packing list details
        const packingListResponse = await fetch(`/api/packaging-list/${packingListId}`);
        const packingListData = await packingListResponse.json();
        
        if (!packingListData.success) {
            throw new Error(packingListData.message || 'Failed to fetch packing list');
        }
        
        const packingList = packingListData.data;
        
        // Extract packing details from JSON
        const packingDetails = JSON.parse(packingList.packingDetailsJson || '[]');
        
        // Create a workbook
        const wb = XLSX.utils.book_new();
        
        // Create a worksheet for the main data
        const mainData = [];
        const mainHeaders = [];
        const mainRow = {};
        
        // Process mappings to separate main fields and packing detail fields
        const mainFields = [];
        const detailFields = [];
        
        mappings.forEach(mapping => {
            const fieldName = mapping.databaseField;
            
            // Check if this is a packing detail field
            const isDetailField = [
                "Sl", "NoOfCarton", "Start", "End", "SizeName", "Ratio", "ArticleNo", 
                "PcsPack", "PacCarton", "OrderQty", "TotalPcs", "TotalPacs", "GWt", 
                "NWt", "TotalWt", "L", "W", "H", "CBM"
            ].includes(fieldName);
            
            if (isDetailField) {
                detailFields.push(mapping);
            } else {
                mainFields.push(mapping);
                mainHeaders.push(mapping.excelColumn);
                
                // Convert PascalCase to camelCase for JavaScript
                const camelCaseField = fieldName.charAt(0).toLowerCase() + fieldName.slice(1);
                mainRow[mapping.excelColumn] = packingList[camelCaseField] || '';
            }
        });
        
        // Add main data headers
        mainData.push(mainHeaders);
        
        // Add main data row
        const mainDataRow = mainHeaders.map(header => mainRow[header] || '');
        mainData.push(mainDataRow);
        
        // Create main worksheet
        const mainWs = XLSX.utils.aoa_to_sheet(mainData);
        XLSX.utils.book_append_sheet(wb, mainWs, "Main Data");
        
        // Create a worksheet for packing details if there are detail fields
        if (detailFields.length > 0) {
            const detailData = [];
            const detailHeaders = detailFields.map(field => field.excelColumn);
            detailData.push(detailHeaders);
            
            // Add packing details rows
            packingDetails.forEach(detail => {
                const row = [];
                detailFields.forEach(field => {
                    const fieldName = field.databaseField;
                    // Convert PascalCase to camelCase for JavaScript
                    const camelCaseField = fieldName.charAt(0).toLowerCase() + fieldName.slice(1);
                    row.push(detail[camelCaseField] || '');
                });
                detailData.push(row);
            });
            
            const detailWs = XLSX.utils.aoa_to_sheet(detailData);
            XLSX.utils.book_append_sheet(wb, detailWs, "Packing Details");
        }
        
        // Generate filename
        const fileName = `${packingList.company || 'PackingList'}_${packingListId}.xlsx`;
        
        // Save the file
        XLSX.writeFile(wb, fileName);
        
        showToast('Download completed successfully!', 'success');
    } catch (error) {
        console.error('Error generating Excel:', error);
        showToast('Error generating Excel file. Please try again.', 'error');
    }
}

// Initialize page
 $(document).ready(function() {
    // Load declaration questions
    loadDeclarationQuestions();
    
    // Check if we're editing an existing record
    const id = getUrlParameter('id');
    if (id) {
        loadPackagingList(id);
    }
    
    // Initialize summary
    updateSummary();
    
    // Handle form submission
    $('#packingListForm').on('submit', function(e) {
        e.preventDefault();
        saveForm();
    });
});

// Modal functionality
document.addEventListener('DOMContentLoaded', () => {
    const openBtnPackingDec = document.getElementById('openModalPackingDecSettings');
    const openBtnTemplate = document.getElementById('openModalTemplate');
    const modal = document.getElementById('pageModal');
    const iframe = document.getElementById('pageFrame');
    const closeBtn = document.getElementById('closeModalPackingDec');
    const closeModalBtn = document.getElementById('closeModalBtn');

    // Log elements to verify they exist
    console.log({
        openBtnPackingDec,
        openBtnTemplate,
        modal,
        iframe,
        closeBtn
    });

    if (!openBtnPackingDec || !modal || !iframe || !closeBtn) {
        console.error('One or more required elements are missing.');
        return;
    }

    openBtnPackingDec.addEventListener('click', () => {
        console.log('Packing Declaration Settings clicked');
        OpenModal('decsetting');
    });

    openBtnTemplate.addEventListener('click', () => {
        console.log('Template Settings clicked');
        OpenModal('templatesettings');
    });
 
    function OpenModal(e) {
        console.log('OpenModal called with:', e);
        if (e === 'decsetting') {
            iframe.src = '../packingdeclarationSettings/PackingDeclarationSettings.html';
            modal.style.display = 'flex';
        } else if (e === 'templatesettings') {
            // Pass the packing list ID to the template settings page
            const packingListId = getPackingListId();
            const url = packingListId 
                ? `../../TemplateSettings/CreateTemplate.html?packingListId=${packingListId}`
                : '../../TemplateSettings/CreateTemplate.html';
            iframe.src = url;
            modal.style.display = 'flex';
        }
    }

    closeBtn.addEventListener('click', () => {
        console.log('Close button clicked');
        modal.style.display = 'none';
        iframe.src = '';
    });

    if (closeModalBtn) {
        closeModalBtn.addEventListener('click', () => {
            console.log('Close modal button clicked');
            modal.style.display = 'none';
            iframe.src = '';
        });
    }

    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            console.log('Modal background clicked');
            modal.style.display = 'none';
            iframe.src = '';
        }
    });
});