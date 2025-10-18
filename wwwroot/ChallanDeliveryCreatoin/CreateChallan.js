

        const now = new Date();

        // Format date as yyyy-mm-dd
        const date = now.toISOString().split('T')[0];

        // Format time as hh:mm (24-hour format)
        const time = now.toTimeString().split(' ')[0].slice(0, 5);

        // Set values to inputs
        document.getElementById('inDate').value = date;
        document.getElementById('inTime').value = time;

        let cbmItems = [];
        const deliveryChallanApiBaseUrl = '/api/deliverychallan';
        const truckApiBaseUrl = '/api/truck';
        let currentAction = null;
        let upadtemode = false;
        let truckRowCount = 0;
        let allTrucks = []; // Store all trucks from database

        // Show loader with custom text
        function showLoader(text = 'Processing...') {
            document.getElementById('loaderText').textContent = text;
            document.getElementById('loaderOverlay').classList.add('active');
        }

         function isValidDate(dateString) {
            if (dateString == null) return false;
            const date = new Date(dateString);
            return !isNaN(date.getTime()) && date.getFullYear() > 1900;
        }
        // Hide loader
        function hideLoader() {
            document.getElementById('loaderOverlay').classList.remove('active');
        }

        function generateChallanNo() {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0'); // 01-12
        const day = String(now.getDate()).padStart(2, '0');       // 01-31
        const hours = String(now.getHours()).padStart(2, '0');    // 00-23
        const minutes = String(now.getMinutes()).padStart(2, '0');
        const seconds = String(now.getSeconds()).padStart(2, '0');

        // Format: CH-YYYYMMDD-HHMMSS
        return `CH-${year}-${month}${day}${hours}${minutes}${seconds}`;
    }

        // Load existing challans on page load
        $(document).ready(function() {
            loadChallans();
            loadTrucks();
             const challanInput = document.getElementById('challanNo');
             challanInput.value = generateChallanNo();
            
            // Auto-populate transport company and other fields when truck is selected
            $('#existingTruckNo').change(function() {
                const selectedOption = $(this).find('option:selected');
                const company = selectedOption.data('company');
                const truckId = selectedOption.data('id');
                
                $('#transportCompany').val(company || '');
                
                // If a truck is selected, populate related fields
                if (truckId) {
                    populateTruckDetails(truckId);
                }
            });

            // Add truck row button click
            $('#addTruckRowBtn').click(addTruckRow);
            
            // Load existing trucks button click
            $('#loadExistingTrucksBtn').click(loadExistingTrucks);
            
            // Save all trucks button click
            $('#saveAllTrucksBtn').click(saveAllTrucks);
            
            // Modal shown event - add initial row if empty
            $('#truckModal').on('shown.bs.modal', function() {
                if (truckRowCount === 0) {
                    addTruckRow();
                }
            });
            
            // Modal hidden event - reset form
            $('#truckModal').on('hidden.bs.modal', function() {
                resetTruckForm();
            });
        });

        // Populate truck details when a truck is selected
        function populateTruckDetails(truckId) {
            const truck = allTrucks.find(t => t.id === truckId);
            if (truck) {
                $('#licenseNo').val(truck.truckLicense || '');
                // You can populate other fields as needed
                // For example, if you have fields for driver name, driver license, etc.
                // $('#driverName').val(truck.driverName || '');
                // $('#driverLicense').val(truck.driverLicense || '');
                // $('#driverPhone').val(truck.driverPhone || '');
            }
        }

        // Add a new truck row to the table
        function addTruckRow(truck = null) {
            truckRowCount++;
            const tbody = document.getElementById('truckTableBody');
            const row = document.createElement('tr');
            
            if (truck && truck.id) {
                // This is an existing truck
                row.id = `truckRow${truckRowCount}`;
                row.className = 'existing-truck-row';
                row.setAttribute('data-id', truck.id);
                row.innerHTML = `
                    <td><input type="text" class="truck-input" value="${truck.truckNumber || ''}" disabled></td>
                    <td><input type="text" class="truck-input" value="${truck.truckLicense || ''}" disabled></td>
                    <td><input type="text" class="truck-input" value="${truck.transportCompanyName || ''}" disabled></td>
                    <td><input type="text" class="truck-input" value="${truck.driverName || ''}" disabled></td>
                    <td><input type="text" class="truck-input" value="${truck.driverLicense || ''}" disabled></td>
                    <td><input type="text" class="truck-input" value="${truck.driverPhone || ''}" disabled></td>
                    <td>
                        <div class="action-buttons">
                            <button type="button" class="edit-truck-btn" onclick="editTruckRow('${truck.id}')" title="Edit Truck">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button type="button" class="remove-truck-btn" onclick="removeTruckRow('${truck.id}', true)" title="Delete Truck">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    </td>
                `;
            } else {
                // This is a new truck
                row.id = `truckRow${truckRowCount}`;
                row.innerHTML = `
                    <td><input type="text" class="truck-input" placeholder="Truck Number" required></td>
                    <td><input type="text" class="truck-input" placeholder="Truck License" required></td>
                    <td><input type="text" class="truck-input" placeholder="Transport Company" required></td>
                    <td><input type="text" class="truck-input" placeholder="Driver Name" required></td>
                    <td><input type="text" class="truck-input" placeholder="Driver License" required></td>
                    <td><input type="text" class="truck-input" placeholder="Driver Phone" required></td>
                    <td>
                        <div class="action-buttons">
                            <button type="button" class="remove-truck-btn" onclick="removeTruckRow(${truckRowCount})" title="Remove Row">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    </td>
                `;
            }
            
            tbody.appendChild(row);
            
            // Hide empty message
            document.getElementById('emptyTruckMessage').style.display = 'none';
        }

        // Edit a truck row
        function editTruckRow(truckId) {
            const row = document.querySelector(`tr[data-id="${truckId}"]`);
            if (!row) return;
            
            const truck = allTrucks.find(t => t.id === truckId);
            if (!truck) return;
            
            // Remove the existing row
            row.remove();
            
            // Add a new editable row with the truck data
            truckRowCount++;
            const tbody = document.getElementById('truckTableBody');
            const newRow = document.createElement('tr');
            newRow.id = `truckRow${truckRowCount}`;
            newRow.setAttribute('data-editing-id', truckId);
            newRow.innerHTML = `
                <td><input type="text" class="truck-input" value="${truck.truckNumber || ''}" required></td>
                <td><input type="text" class="truck-input" value="${truck.truckLicense || ''}" required></td>
                <td><input type="text" class="truck-input" value="${truck.transportCompanyName || ''}" required></td>
                <td><input type="text" class="truck-input" value="${truck.driverName || ''}" required></td>
                <td><input type="text" class="truck-input" value="${truck.driverLicense || ''}" required></td>
                <td><input type="text" class="truck-input" value="${truck.driverPhone || ''}" required></td>
                <td>
                    <div class="action-buttons">
                        <button type="button" class="remove-truck-btn" onclick="removeTruckRow(${truckRowCount})" title="Remove Row">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            `;
            
            tbody.appendChild(newRow);
        }

        // Load existing trucks into the modal
        function loadExistingTrucks() {
            showLoader('Loading existing trucks...');
            
            fetch(truckApiBaseUrl)
                .then(response => {
                    if (!response.ok) throw new Error('Failed to fetch trucks');
                    return response.json();
                })
                .then(trucks => {
                    // Clear existing rows
                    document.getElementById('truckTableBody').innerHTML = '';
                    truckRowCount = 0;
                    
                    // Add each truck as a disabled row
                    trucks.forEach(truck => {
                        addTruckRow(truck);
                    });
                    
                    hideLoader();
                })
                .catch(error => {
                    console.error('Error loading existing trucks:', error);
                    hideLoader();
                    showConfirmationDialog('Failed to load existing trucks. Please try again.');
                });
        }

        // Remove a truck row from the table
        function removeTruckRow(rowId, isExisting = false) {
            if (isExisting) {
                // This is an existing truck, confirm deletion
                currentAction = () => {
                    showLoader('Deleting truck...');
                    
                    fetch(`${truckApiBaseUrl}/${rowId}`, { method: 'DELETE' })
                        .then(response => {
                            if (response.ok) {
                                hideLoader();
                                showConfirmationDialog('Truck deleted successfully!');
                                
                                // Remove the row from the table
                                const row = document.querySelector(`tr[data-id="${rowId}"]`);
                                if (row) {
                                    row.remove();
                                    truckRowCount--;
                                    
                                    // Show empty message if no rows left
                                    if (truckRowCount === 0) {
                                        document.getElementById('emptyTruckMessage').style.display = 'block';
                                    }
                                }
                                
                                // Reload trucks dropdown
                                loadTrucks();
                            } else {
                                throw new Error('Failed to delete truck');
                            }
                        })
                        .catch(error => {
                            console.error('Error deleting truck:', error);
                            hideLoader();
                            showConfirmationDialog('Failed to delete truck. Please try again.');
                        });
                };
                
                showConfirmationDialog('Are you sure you want to delete this truck? This action cannot be undone.', true);
            } else {
                // This is a new truck row, just remove it
                const row = document.getElementById(`truckRow${rowId}`);
                if (row) {
                    row.remove();
                    truckRowCount--;
                    
                    // Show empty message if no rows left
                    if (truckRowCount === 0) {
                        document.getElementById('emptyTruckMessage').style.display = 'block';
                    }
                }
            }
        }

        // Save all trucks from the table
        function saveAllTrucks() {
            const trucks = [];
            const updatedTrucks = [];
            const rows = document.querySelectorAll('#truckTableBody tr');
            
            // Validate and collect truck data
            let isValid = true;
            rows.forEach(row => {
                const inputs = row.querySelectorAll('.truck-input');
                const isExisting = row.hasAttribute('data-editing-id');
                
                if (isExisting) {
                    // This is an existing truck being edited
                    const truckId = row.getAttribute('data-editing-id');
                    const truckData = {
                        id: truckId,
                        truckNumber: inputs[0].value.trim(),
                        truckLicense: inputs[1].value.trim(),
                        transportCompanyName: inputs[2].value.trim(),
                        driverName: inputs[3].value.trim(),
                        driverLicense: inputs[4].value.trim(),
                        driverPhone: inputs[5].value.trim()
                    };
                    
                    // Validate all fields
                    if (!truckData.truckNumber || !truckData.truckLicense || !truckData.transportCompanyName ||
                        !truckData.driverName || !truckData.driverLicense || !truckData.driverPhone) {
                        isValid = false;
                        inputs.forEach(input => {
                            if (!input.value.trim()) {
                                input.style.borderColor = '#dc3545';
                            } else {
                                input.style.borderColor = '#ced4da';
                            }
                        });
                    } else {
                        updatedTrucks.push(truckData);
                    }
                } else if (!row.hasAttribute('data-id')) {
                    // This is a new truck
                    const truckData = {
                        truckNumber: inputs[0].value.trim(),
                        truckLicense: inputs[1].value.trim(),
                        transportCompanyName: inputs[2].value.trim(),
                        driverName: inputs[3].value.trim(),
                        driverLicense: inputs[4].value.trim(),
                        driverPhone: inputs[5].value.trim()
                    };
                    
                    // Validate all fields
                    if (!truckData.truckNumber || !truckData.truckLicense || !truckData.transportCompanyName ||
                        !truckData.driverName || !truckData.driverLicense || !truckData.driverPhone) {
                        isValid = false;
                        inputs.forEach(input => {
                            if (!input.value.trim()) {
                                input.style.borderColor = '#dc3545';
                            } else {
                                input.style.borderColor = '#ced4da';
                            }
                        });
                    } else {
                        trucks.push(truckData);
                    }
                }
            });
            
            if (!isValid) {
                showConfirmationDialog('Please fill all required fields in all truck rows.');
                return;
            }
            
            if (trucks.length === 0 && updatedTrucks.length === 0) {
                showConfirmationDialog('Please add at least one truck or make changes to existing trucks.');
                return;
            }
            
            showLoader('Saving trucks...');
            
            // First, save new trucks if any
            if (trucks.length > 0) {
                const requestData = {
                    Trucks: trucks
                };
                
                fetch(`${truckApiBaseUrl}/list`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(requestData)
                })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => { throw new Error(`HTTP error! Status: ${response.status}, Message: ${text}`); });
                    }
                    return response.json();
                })
                .then(data => {
                    // Then update existing trucks if any
                    if (updatedTrucks.length > 0) {
                        return updateExistingTrucks(updatedTrucks);
                    } else {
                        return Promise.resolve(data);
                    }
                })
                .then(data => {
                    hideLoader();
                    showConfirmationDialog(`${trucks.length + updatedTrucks.length} truck(s) saved successfully!`);
                    
                    // Close modal
                    const modal = bootstrap.Modal.getInstance(document.getElementById('truckModal'));
                    modal.hide();
                    
                    // Reload trucks
                    loadTrucks();
                })
                .catch(error => {
                    console.error('Error:', error);
                    hideLoader();
                    showConfirmationDialog('Failed to save trucks. Check console for details. Error: ' + error.message);
                });
            } else if (updatedTrucks.length > 0) {
                // Only update existing trucks
                updateExistingTrucks(updatedTrucks)
                    .then(data => {
                        hideLoader();
                        showConfirmationDialog(`${updatedTrucks.length} truck(s) updated successfully!`);
                        
                        // Close modal
                        const modal = bootstrap.Modal.getInstance(document.getElementById('truckModal'));
                        modal.hide();
                        
                        // Reload trucks
                        loadTrucks();
                    })
                    .catch(error => {
                        console.error('Error:', error);
                        hideLoader();
                        showConfirmationDialog('Failed to update trucks. Check console for details. Error: ' + error.message);
                    });
            } else {
                hideLoader();
                showConfirmationDialog('No changes to save.');
            }
        }

        // Update existing trucks
        function updateExistingTrucks(trucks) {
            const updatePromises = trucks.map(truck => {
                return fetch(`${truckApiBaseUrl}/${truck.id}`, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(truck)
                })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => { throw new Error(`HTTP error! Status: ${response.status}, Message: ${text}`); });
                    }
                    return response.json();
                });
            });
            
            return Promise.all(updatePromises);
        }

        // Reset truck form
        function resetTruckForm() {
            document.getElementById('truckTableBody').innerHTML = '';
            truckRowCount = 0;
            document.getElementById('emptyTruckMessage').style.display = 'block';
        }

        // Load trucks from database
        function loadTrucks() {
            showLoader('Loading trucks...');
            
            fetch(truckApiBaseUrl)
                .then(response => {
                    if (!response.ok) throw new Error('Failed to fetch trucks');
                    return response.json();
                })
                .then(trucks => {
                    // Store all trucks for later use
                    allTrucks = trucks;
                    
                    const select = document.getElementById('existingTruckNo');
                    // Clear existing options except the first one
                    select.innerHTML = '<option value="">Select existing truck</option>';
                    
                    trucks.forEach(truck => {
                        const option = document.createElement('option');
                        option.value = truck.truckNumber;
                        option.textContent = truck.truckNumber;
                        option.setAttribute('data-company', truck.transportCompanyName);
                        option.setAttribute('data-id', truck.id);
                        select.appendChild(option);
                    });
                    
                    hideLoader();
                })
                .catch(error => {
                    console.error('Error loading trucks:', error);
                    hideLoader();
                    
                   
                });
        }

        // Submit/Create or Update Challan
        document.getElementById('submitChallanBtn').addEventListener('click', function() {
            // Get the ID value or generate empty GUID if it's empty
            const challanId = document.getElementById('challanId').value.trim();
            
            const challanData = {
                id: challanId || '00000000-0000-0000-0000-000000000000',
                challanNo: document.getElementById('challanNo').value.trim(),
                existingTruckNo: document.getElementById('existingTruckNo').value.trim(),
                // hireTruckNo: document.getElementById('hireTruckNo').value.trim(),
                inDate: document.getElementById('inDate').value,
                inTime: document.getElementById('inTime').value,
                to: document.getElementById('to').value.trim(),
                driverName: document.getElementById('driverName').value.trim(),
                outDate: document.getElementById('outDate').value,
                outTime: document.getElementById('outTime').value,
                address: document.getElementById('address').value.trim(),
                licenseNo: document.getElementById('licenseNo').value.trim(),
                truckCBM: document.getElementById('truckCBM').value.trim(),
                depotName: document.getElementById('depotName').value.trim(),
                mobileNo: document.getElementById('mobileNo').value.trim(),

                unit: document.getElementById('unit').value,
                quantity: document.getElementById('quantity').value,
                remarks: document.getElementById('remarks').value,
                descriptions: document.getElementById('descriptions').value,
                agdl: document.getElementById('agdlCBM').value,


                cbmItems: cbmItems.length > 0 ? cbmItems : [{
                    id: '00000000-0000-0000-0000-000000000000',
                    companyWiseCBM: "Default",
                    cbm: "0",
                    rentAmount: "0"
                }], // Add default CBM item if empty
                transportCompany: document.getElementById('transportCompany').value.trim(),
                lockNo: document.getElementById('lockNo').value.trim() || "",
                // rentedAmount: document.getElementById('rentedAmount').value.trim() || "",
                poNo: "" // Added as per DTO but not in form
            };

            if (!challanData.challanNo || !challanData.inDate || !challanData.to || !challanData.driverName  || !challanData.address || !challanData.licenseNo || !challanData.truckCBM || !challanData.depotName || !challanData.mobileNo || !challanData.transportCompany) {
                showConfirmationDialog('Please fill all required fields.');
                return;
            }

            currentAction = () => {
                showLoader(challanId ? 'Updating challan...' : 'Creating challan...');
                
                const method = challanId ? 'PUT' : 'POST';
                const url = challanId ? `${deliveryChallanApiBaseUrl}/${challanId}` : deliveryChallanApiBaseUrl;
                console.log("challanData", challanData);
                // The API expects the data directly, not wrapped in a "dto" object
                fetch(url, {
                    method: method,
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(challanData)
                })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => { throw new Error(`HTTP error! Status: ${response.status}, Message: ${text}`); });
                    }
                    return response.json();
                })
                .then(data => {
                    hideLoader();
                    showConfirmationDialog(`Delivery Challan ${challanId ? 'updated' : 'created'} successfully!`);
                    loadChallans();
                    clearForm();
                })
                .catch(error => {
                    console.error('Error:', error);
                    hideLoader();
                    showConfirmationDialog('Failed to process Delivery Challan. Check console for details. Error: ' + error.message);
                });
            };

            showConfirmationDialog(`Are you sure you want to ${challanId ? 'update' : 'create'} this Delivery Challan?`, true);
        });

        // Load all challans
        function loadChallans() {
            showLoader('Loading challans...');
            
            fetch(deliveryChallanApiBaseUrl)
                .then(response => {
                    if (!response.ok) throw new Error('Failed to fetch challans');
                    return response.json();
                })
                .then(challans => {
                    console.log("data fetchd",challans)
                    const tbody = document.getElementById('challansTable').querySelector('tbody');
                    tbody.innerHTML = '';
                    console.log("dadda",challans)
                    challans.forEach(challan => {
                        const row = document.createElement('tr');
                        row.innerHTML = `
                            <td>${challan.challanNo}</td>
                            <td>${challan.existingTruckNo || challan.hireTruckNo || 'N/A'}</td>
                            <td>${challan.to}</td>
                            <td>${new Date(challan.inDate).toLocaleDateString()}</td>
        
                            <td>${isValidDate(challan.outDate) ? new Date(challan.outDate).toLocaleDateString() : '- -'}</td>
                        

 
                            <td>
                                <div class="dropdown">
                                    <button class="btn btn-sm btn-secondary dropdown-toggle" type="button" id="actionDropdown${challan.id}" data-bs-toggle="dropdown" aria-expanded="false">
                                        Actions
                                    </button>
                                    <ul class="dropdown-menu" aria-labelledby="actionDropdown${challan.id}">
                                        <li><a class="dropdown-item" href="#" onclick="editChallan('${challan.id}')"><i class="fas fa-edit text-warning"></i> Edit</a></li>
                                        <li><a class="dropdown-item" href="#" onclick="confirmDelete('${challan.id}')"><i class="fas fa-trash text-danger"></i> Delete</a></li>
                                    </ul>
                                </div>
                            </td>
                        `;
                        tbody.appendChild(row);
                    });
                    
                    hideLoader();
                })
                .catch(error => {
                    console.error('Error loading challans:', error);
                    hideLoader();
                    // Add dummy data if API fails
                    const tbody = document.getElementById('challansTable').querySelector('tbody');
                    tbody.innerHTML = '';
                });
        }

        // Edit challan
        function editChallan(id) {
            showLoader('Loading challan for editing...');
            
            fetch(`${deliveryChallanApiBaseUrl}/${id}`)
                .then(response => {
                    if (!response.ok) throw new Error('Failed to fetch challan');
                    return response.json();
                })
                .then(challan => {
                    upadtemode=true;

                    console.log("data got",challan)
                 
                    $('#submitChallanBtn').html('<i class="fas fa-save me-2"></i>Update Challan');
                    console.log("update called data", challan);
                    document.getElementById('challanId').value = challan.id;
                    document.getElementById('challanNo').value = challan.challanNo;
                    document.getElementById('existingTruckNo').value = challan.existingTruckNo || '';
                    // document.getElementById('hireTruckNo').value = challan.hireTruckNo || '';
                    document.getElementById('inDate').value = challan.inDate.split('T')[0];
                    document.getElementById('inTime').value = challan.inTime;
                    document.getElementById('to').value = challan.to;
                    document.getElementById('driverName').value = challan.driverName;
                    // document.getElementById('outDate').value =  new Date(challan.outDate).toLocaleDateString();
                    document.getElementById('outDate').value = challan.outDate.split(' ')[0];

                    document.getElementById('outTime').value = challan.outTime;
                    document.getElementById('address').value = challan.address;
                    document.getElementById('licenseNo').value = challan.licenseNo;
                    document.getElementById('truckCBM').value = challan.truckCBM;
                    document.getElementById('depotName').value = challan.depotName;
                    document.getElementById('mobileNo').value = challan.mobileNo;
                    document.getElementById('transportCompany').value = challan.transportCompany;
                    document.getElementById('lockNo').value = challan.lockNo || '';

                    document.getElementById('unit').value=challan.unit;
                    document.getElementById('quantity').value=challan.quantity;
                    document.getElementById('remarks').value=challan.remarks;
                    document.getElementById('descriptions').value=challan.descriptions;
                    document.getElementById('agdlCBM').value=challan.agdl;
                    // document.getElementById('rentedAmount').value = challan.rentedAmount || '';

                    const tbody = document.getElementById('cbmTable').querySelector('tbody');
                    tbody.innerHTML = '';
                    cbmItems = challan.cbmItems || [];
                    challan.cbmItems.forEach(item => {
                        const row = document.createElement('tr');
                        row.innerHTML = `
                            <td>${item.companyWiseCBM}</td>
                            <td>${item.cbm}</td>
                            <td>${item.rentAmount}</td>
                            <td><button type="button" class="btn btn-sm btn-danger remove-cbm">Remove</button></td>
                        `;
                        tbody.appendChild(row);
                    });
                    
                    hideLoader();
                })
                .catch(error => {
                    hideLoader();
                    console.error('Error loading challan for edit:', error);
                });
        }

        // Confirm delete using Bootstrap modal
        function confirmDelete(id) {
            currentAction = () => {
                showLoader('Deleting challan...');
                
                fetch(`${deliveryChallanApiBaseUrl}/${id}`, { method: 'DELETE' })
                    .then(response => {
                        if (response.ok) {
                            hideLoader();
                            showConfirmationDialog('Delivery Challan deleted successfully!');
                            loadChallans();
                        } else {
                            throw new Error('Failed to delete challan');
                        }
                    })
                    .catch(error => {
                        console.error('Error deleting challan:', error);
                        hideLoader();
                        showConfirmationDialog('Failed to delete Delivery Challan. Check console for details.');
                    });
            };

            showConfirmationDialog('Are you sure you want to delete this challan?', true);
        }

        // Show confirmation dialog
        function showConfirmationDialog(message, isConfirmation = false) {
            const modal = new bootstrap.Modal(document.getElementById('confirmationModal'));
            document.getElementById('confirmationModalBody').textContent = message;
            
            if (isConfirmation) {
                document.getElementById('confirmButton').style.display = 'block';
                document.getElementById('confirmButton').onclick = function() {
                    modal.hide();
                    if (currentAction) {
                        currentAction();
                        currentAction = null;
                    }
                };
            } else {
                document.getElementById('confirmButton').style.display = 'none';
            }
            
            modal.show();
        }

        // Clear form
        function clearForm() {
            document.getElementById('challanId').value = '';
            document.getElementById('challanNo').value = '';
            document.getElementById('existingTruckNo').value = '';
            // document.getElementById('hireTruckNo').value = '';
            document.getElementById('inDate').value = '2025-10-07';
            document.getElementById('inTime').value = '19:06';
            document.getElementById('to').value = '';
            document.getElementById('driverName').value = '';
            document.getElementById('outDate').value = '2025-10-07';
            document.getElementById('outTime').value = '19:06';
            document.getElementById('address').value = '';
            document.getElementById('licenseNo').value = '';
            document.getElementById('truckCBM').value = '';
            document.getElementById('depotName').value = '';
            document.getElementById('mobileNo').value = '';
            document.getElementById('transportCompany').value = '';
            document.getElementById('lockNo').value = '';
            // document.getElementById('rentedAmount').value = '';
            cbmItems = [];
            $('#submitChallanBtn').html('<i class="fas fa-save me-2"></i>Submit Challan');
            upadtemode = false;
        }
