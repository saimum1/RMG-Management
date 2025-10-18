
        const gatepassApiBaseUrl =  '/api/gatepass';
        const challanApiBaseUrl = '/api/deliverychallan';
        const truckApiBaseUrl = '/api/truck';

        let currentAction = null;
        let updateMode = false;
        let currentGatepassId = null;
        let allTrucks = [];
        let selectedChallanData=[];
        let selectedTruck;

        // Show loader with custom text
        function showLoader(text = 'Processing...') {
            document.getElementById('loaderText').textContent = text;
            document.getElementById('loaderOverlay').classList.add('active');
        }

        // Hide loader
        function hideLoader() {
            document.getElementById('loaderOverlay').classList.remove('active');
        }


        function generategatepassNo() {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0'); // 01-12
        const day = String(now.getDate()).padStart(2, '0');       // 01-31
        const hours = String(now.getHours()).padStart(2, '0');    // 00-23
        const minutes = String(now.getMinutes()).padStart(2, '0');
        const seconds = String(now.getSeconds()).padStart(2, '0');

        // Format: CH-YYYYMMDD-HHMMSS
        return `GP-${year}-${month}${day}${hours}${minutes}${seconds}`;
    }
        // Load existing gatepasses on page load
        $(document).ready(function() {
            loadGatepasses();
            loadTrucks();
             const gatepass = document.getElementById('gatePassNo');
              gatepass.value = generategatepassNo();
              
            // Load challans and auto-populate transport company when truck is selected
            $('#truckNo').change(function() {
                const selectedOption = $(this).find('option:selected');
                console.log("selected data", selectedOption.data());
                const company = selectedOption.data('company');
                
                // Auto-populate transport company
                $('#transportCompany').val(company || '');
                const truckId = selectedOption.data('id');
                if (truckId) {
                    populateTruckDetails(truckId);
                }
                
                // Load challans for the selected truck
                loadChallansForTruck($(this).val());
            });
        });



          function populateTruckDetails(truckId) {
            const truck = allTrucks.find(t => t.id === truckId);
            if (truck) {
                selectedTruck = truck;
                console.log("alltruckinfo_",truck);
                $('#driverLicense').val(truck.driverLicense || '');
                $('#driverName').val(truck.driverName || '');
                $('#driverPhone').val(truck.driverPhone || '');
                // You can populate other fields as needed
                // For example, if you have fields for driver name, driver license, etc.
                // $('#driverName').val(truck.driverName || '');
                // $('#driverLicense').val(truck.driverLicense || '');
                // $('#driverPhone').val(truck.driverPhone || '');
            }
        }


        // Load all gatepasses
        function loadGatepasses() {
            showLoader('Loading gatepasses...');
            
            fetch(gatepassApiBaseUrl)
                .then(response => {
                    if (!response.ok) throw new Error('Failed to fetch gatepasses');
                    return response.json();
                })
                .then(gatepasses => {
                    console.log("gatepass",gatepasses)
                    const tbody = document.getElementById('gatepassesTable').querySelector('tbody');
                    tbody.innerHTML = '';
                    gatepasses.forEach(gatepass => {
                        const row = document.createElement('tr');
                        row.innerHTML = `
                            <td>${gatepass.gatePassNo}</td>
                            <td>${new Date(gatepass.date).toLocaleDateString()}</td>
                            <td>${gatepass.companyName}</td>
                            <td>${gatepass.issuedTo}</td>
                            <td>${gatepass.truckNo || 'N/A'}</td>
                            <td>
                                <div class="dropdown">
                                    <button class="btn btn-sm btn-secondary dropdown-toggle" type="button" id="actionDropdown${gatepass.id}" data-bs-toggle="dropdown" aria-expanded="false">
                                        Actions
                                    </button>
                                    <ul class="dropdown-menu" aria-labelledby="actionDropdown${gatepass.id}">
                                        <li><a class="dropdown-item" href="#" onclick="viewGatepass('${gatepass.id}')"><i class="fas fa-eye text-primary"></i> View</a></li>
                                        <li><a class="dropdown-item" href="#" onclick="editGatepass('${gatepass.id}')"><i class="fas fa-edit text-warning"></i> Edit</a></li>
                                        <li><a class="dropdown-item" href="#" onclick="confirmDelete('${gatepass.id}')"><i class="fas fa-trash text-danger"></i> Delete</a></li>
                                    </ul>
                                </div>
                            </td>
                        `;
                        tbody.appendChild(row);
                    });
                    
                    hideLoader();
                })
                .catch(error => {
                    console.error('Error loading gatepasses:', error);
                    hideLoader();
                });
        }

        // const getChllansForSpecificTrucks async (params) => {
            
        // }
        const getChllansForSpecificTrucks = async (truckNo) => {
             if (!truckNo) {
                document.getElementById('challansList').innerHTML = '<p class="text-muted">Select a truck to view associated delivery challans</p>';
                return;
            }

            return await fetch(challanApiBaseUrl)
                .then(response => {
                    if (!response.ok) throw new Error('Failed to fetch challans');
                    return response.json();
                })
                .then(challans => {
                    const filteredChallans = challans.filter(challan => 
                        challan.existingTruckNo === truckNo || challan.hireTruckNo === truckNo
                    );
                    
                    console.log("datafikgtere",truckNo, filteredChallans);
                   
                    return filteredChallans;
                   
                })
                .catch(error => {
                    console.error('Error loading challans:', error);
                    return []
                });
        }
        // Load challans for a specific truck
        function loadChallansForTruck(truckNo) {
            if (!truckNo) {
                document.getElementById('challansList').innerHTML = '<p class="text-muted">Select a truck to view associated delivery challans</p>';
                return;
            }

            showLoader('Loading challans for truck...');
            
            // In a real application, you would filter by truck number on the server side
            // For this example, we'll simulate filtering on the client side
            fetch(challanApiBaseUrl)
                .then(response => {
                    if (!response.ok) throw new Error('Failed to fetch challans');
                    return response.json();
                })
                .then(challans => {
                    const challansList = document.getElementById('challansList');
                    challansList.innerHTML = '';
                    
                    // Filter challans by truck number
                    const filteredChallans = challans.filter(challan => 
                        challan.existingTruckNo === truckNo || challan.hireTruckNo === truckNo
                    );
                    
                    if (filteredChallans.length === 0) {
                        challansList.innerHTML = '<p class="text-muted">No delivery challans found for this truck</p>';
                        hideLoader();
                        return;
                    }
                    selectedChallanData=filteredChallans;
                    filteredChallans.forEach(challan => {
                       
                        const challanCard = document.createElement('div');
                        challanCard.className = 'challan-card';
                        challanCard.innerHTML = `
                            <div class="challan-card-header">
                                <div class="challan-card-title">${challan.challanNo}</div>
                                <div>
                                  
                                </div>
                            </div>
                            <div class="challan-card-body">
                                <div class="challan-card-item">
                                    <span class="challan-card-label">Truck No.</span>
                                    <span class="challan-card-value">${challan.existingTruckNo || challan.hireTruckNo || 'N/A'}</span>
                                </div>
                                <div class="challan-card-item">
                                    <span class="challan-card-label">In Date</span>
                                    <span class="challan-card-value">${new Date(challan.inDate).toLocaleDateString()}</span>
                                </div>
                                <div class="challan-card-item">
                                    <span class="challan-card-label">Out Date</span>
                                    <span class="challan-card-value">${new Date(challan.outDate).toLocaleDateString()}</span>
                                </div>
                                
                                <div class="challan-card-item">
                                    <span class="challan-card-label">To</span>
                                    <span class="challan-card-value">${challan.to || 'N/A'}</span>
                                </div>
                                <div class="challan-card-item">
                                    <span class="challan-card-label">Transport Company</span>
                                    <span class="challan-card-value">${challan.transportCompany || 'N/A'}</span>
                                </div>
                            </div>
                        `;
                        challansList.appendChild(challanCard);
                    });
                    
                    hideLoader();
                })
                .catch(error => {
                    console.error('Error loading challans:', error);
                    hideLoader();
                    document.getElementById('challansList').innerHTML = '<p class="text-muted">Error loading delivery challans</p>';
                });
        }

        // Select a challan and populate form fields
        function selectChallan(challanId) {
            showLoader('Loading challan details...');
            
            fetch(`${challanApiBaseUrl}/${challanId}`)
                .then(response => {
                    if (!response.ok) throw new Error('Failed to fetch challan');
                    return response.json();
                })
                .then(challan => {
                    // Populate form fields with challan data
                    document.getElementById('remarks').value = `Related to challan: ${challan.challanNo}`;
                    
                    // Show confirmation message
                    showConfirmationDialog('Challan selected successfully. You can now submit the gatepass.');
                    hideLoader();
                })
                .catch(error => {
                    console.error('Error loading challan:', error);
                    hideLoader();
                    showConfirmationDialog('Failed to load challan details.');
                });
        }

        // Submit/Create or Update Gatepass
        document.getElementById('submitGatepassBtn').addEventListener('click', function() {
            // Get the ID value or generate empty GUID if it's empty
            const gatepassId = document.getElementById('gatepassId').value.trim();
            
            const gatepassData = {
                id: gatepassId || '00000000-0000-0000-0000-000000000000',
                gatePassNo: document.getElementById('gatePassNo').value.trim(),
                date: document.getElementById('date').value,
                companyName: document.getElementById('companyName').value.trim(),
                address: document.getElementById('address').value.trim(),
                issuedTo: document.getElementById('issuedTo').value.trim(),
                truckNo: document.getElementById('truckNo').value.trim(),
                driverName: document.getElementById('driverName').value.trim(),
                transportCompany: document.getElementById('transportCompany').value.trim(),
                driverPhone: document.getElementById('driverPhone').value.trim(),
                driverLicense: document.getElementById('driverLicense').value.trim(),
                remarks: document.getElementById('remarks').value.trim()
            };

            if (!gatepassData.gatePassNo || !gatepassData.date || !gatepassData.companyName || 
                !gatepassData.address || !gatepassData.issuedTo || !gatepassData.truckNo || 
                !gatepassData.driverName || !gatepassData.transportCompany || 
                !gatepassData.driverPhone || !gatepassData.driverLicense) {
                showConfirmationDialog('Please fill all required fields.');
                return;
            }

            currentAction = () => {
                showLoader(gatepassId ? 'Updating gatepass...' : 'Creating gatepass...');
                
                const method = gatepassId ? 'PUT' : 'POST';
                const url = gatepassId ? `${gatepassApiBaseUrl}/${gatepassId}` : gatepassApiBaseUrl;

                fetch(url, {
                    method: method,
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(gatepassData)
                })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => { throw new Error(`HTTP error! Status: ${response.status}, Message: ${text}`); });
                    }
                    return response.json();
                })
                .then(data => {
                    hideLoader();
                    showConfirmationDialog(`Gatepass ${gatepassId ? 'updated' : 'created'} successfully!`);
                    loadGatepasses();
                    clearForm();
                })
                .catch(error => {
                    console.error('Error:', error);
                    hideLoader();
                    showConfirmationDialog('Failed to process Gatepass. Check console for details. Error: ' + error.message);
                });
            };

            showConfirmationDialog(`Are you sure you want to ${gatepassId ? 'update' : 'create'} this Gatepass?`, true);
        });

        // View gatepass - SIMPLIFIED WITH ONLY REQUIRED FIELDS
        function viewGatepass(id) {
            showLoader('Loading gatepass details...');
            
            currentGatepassId = id;
            fetch(`${gatepassApiBaseUrl}/${id}`)
                .then(response => {
                    if (!response.ok) throw new Error('Failed to fetch gatepass');
                    return response.json();
                })
                .then( async gatepass => {
                
                    // let chllandDetails=await getChllansForSpecificTrucks(gatepass.truckNo);

                    // console.log("sdadasdsa", chllandDetails);
                    // const content = `
                    //     <button class="download-btn" onclick="downloadGatepassDetails()" title="Download Gatepass Details">
                    //         <i class="fas fa-download"></i>
                    //     </button>
                    //     <div class="simple-gatepass-view" >
                    //         <div class="gatepass-info">
                    //             <div class="gatepass-info-item">
                    //                 <span class="gatepass-info-label">Gate Pass No.</span>
                    //                 <span class="gatepass-info-value">${gatepass.gatePassNo}</span>
                    //             </div>
                    //             <div class="gatepass-info-item">
                    //                 <span class="gatepass-info-label">Date</span>
                    //                 <span class="gatepass-info-value">${new Date(gatepass.date).toLocaleDateString()}</span>
                    //             </div>
                    //             <div class="gatepass-info-item">
                    //                 <span class="gatepass-info-label">Company Name</span>
                    //                 <span class="gatepass-info-value">${gatepass.companyName}</span>
                    //             </div>
                    //             <div class="gatepass-info-item">
                    //                 <span class="gatepass-info-label">Issued To</span>
                    //                 <span class="gatepass-info-value">${gatepass.issuedTo}</span>
                    //             </div>
                    //         </div>
                    //         <div class="qr-code-section">
                    //             <h5><i class="fas fa-qrcode me-2"></i>QR Code</h5>
                    //             <img id="qrCodeImage" src="${gatepassApiBaseUrl}/${id}/qr" alt="QR Code" onload="qrCodeLoaded()">
                    //             <p>Scan to view full details</p>
                    //         </div>
                    //     </div>
                    // `;
                    const content = `
    <button class="download-btn" onclick="downloadGatepassDetails()" title="Download Gatepass Details">
        <i class="fas fa-download"></i>
    </button>

    <div class="gatepass-compact" style="
        display: flex; 
        justify-content: space-between; 
        align-items: flex-start; 
        gap: 1rem; 
        padding: 1rem; 
        border: 1px solid #ddd; 
        border-radius: 8px;
        max-width: 600px;
        margin: auto;
    ">
        <div class="gatepass-details" style="flex: 1;">
            <p><strong>Gate Pass No:</strong> ${gatepass.gatePassNo}</p>
            <p><strong>Date:</strong> ${new Date(gatepass.date).toLocaleDateString()}</p>
            <p><strong>Company:</strong> ${gatepass.companyName}</p>
            <p><strong>Issued To:</strong> ${gatepass.issuedTo}</p>
        </div>

        <div class="qr-section" style="text-align: center;">
            <img id="qrCodeImage" 
                 src="${gatepassApiBaseUrl}/${id}/qr" 
                 alt="QR Code" 
                 onload="qrCodeLoaded()" 
                 style="width: 120px; height: 120px; border: 1px solid #ccc; border-radius: 8px;">
            <p style="font-size: 0.8rem; margin-top: 0.5rem;">Scan to view</p>
        </div>
    </div>
`;

                    document.getElementById('gatepassViewContent').innerHTML = content;
            //         let dataxxx={
            //             gatePassNo: gatepass.gatePassNo,
            //             date: new Date(gatepass.date).toLocaleDateString(),
            //             companyName: gatepass.companyName,
            //             address: gatepass.address,
            //             issuedTo: gatepass.issuedTo,
            //             truckNo: gatepass.truckNo,
            //             driverName: gatepass.driverName,
            //             transportCompany: gatepass.transportCompany,
            //             driverPhone: gatepass.driverPhone,
            //             driverLicense: gatepass.driverLicense,
            //             remarks: gatepass.remarks,
            //             challandataamount:chllandDetails?.length
            //         };
            //         console.log("dataAll",dataxxx)
            //          const jsonString = JSON.stringify(dataxxx);
            //         // const encodedData = encodeURIComponent(jsonString);
            //         let encodedData = btoa(unescape(encodeURIComponent(jsonString)));
            
            // // Create HTML block with QR code container
            // const xhtml = `
            //     <div class="qr-code-section text-center my-3">
            //         <h5><i class="fas fa-qrcode me-2"></i>Gatepass QR Code</h5>
            //         <div class="d-flex justify-content-center">
            //             <div id="qrcode" class="qr-container"></div>
            //         </div>
            //         <p class="mt-3">Scan to view full details</p>
            //         <div class="alert alert-info mt-3">
            //             <small>
            //                 <i class="fas fa-info-circle me-2"></i>
            //                 This QR contains all gatepass information in JSON format
            //             </small>
            //         </div>
            //     </div>
            // `;

            //     const xhtml = `
            //     <div class="qr-code-section text-center my-3">
            //         <h5><i class="fas fa-qrcode me-2"></i>Gatepass QR Code</h5>
            //         <div class="d-flex justify-content-center">
            //              <img id="qrCodeImage" src="https://chart.googleapis.com/chart?cht=qr&chs=200x200&chl=${encodedData}" 
            //  alt="Gatepass QR Code" class="qr-container"/>
            //         </div>
            //         <p class="mt-3">Scan to view full details</p>
            //         <div class="alert alert-info mt-3">
            //             <small>
            //                 <i class="fas fa-info-circle me-2"></i>
            //                 This QR contains all gatepass information in JSON format
            //             </small>
            //         </div>
            //     </div>
            // `;

//             const xhtml = `
// <div class="qr-code-section text-center my-3">
//     <h5><i class="fas fa-qrcode me-2"></i>Gatepass QR Code</h5>
//     <div class="d-flex justify-content-center">
//         <img id="qrCodeImage" src="https://chart.googleapis.com/chart?cht=qr&chs=200x200&chl=${encodedData}" 
//              alt="Gatepass QR Code" class="qr-container"/>
//     </div>
//     <p class="mt-3">Scan to view full details</p>
//     <div class="alert alert-info mt-3">
//         <small>
//             <i class="fas fa-info-circle me-2"></i>
//             This QR contains all gatepass information in JSON format
//         </small>
//     </div>
// </div>
// `;

            // Insert into the container
            // const container = document.getElementById("gatepassViewContent2");
            // container.innerHTML = xhtml;
            
            // Generate QR code after container is created
         
                
            //     console.log("QR code generated successfully");
            // }, 100);
                    
                    const modal = new bootstrap.Modal(document.getElementById('viewGatepassModal'));
                    modal.show();
                    hideLoader();
                })
                .catch(error => {
                    console.error('Error loading gatepass for view:', error);
                    hideLoader();
                    showConfirmationDialog('Failed to load gatepass details.');
                });
        }

        // Flag to track if QR code is loaded
        let qrCodeLoadedFlag = false;

        // Function called when QR code is loaded
        function qrCodeLoaded() {
            qrCodeLoadedFlag = true;
        }

        // Download gatepass details as image
        function downloadGatepassDetails() {
            showLoader('Preparing download...');
            
            const element = document.getElementById('gatepassViewContent');
            
            // Check if QR code is loaded
            if (!qrCodeLoadedFlag) {
                hideLoader();
                showConfirmationDialog('QR code is still loading. Please wait a moment and try again.');
                return;
            }
            
            // Temporarily hide the download button
            const downloadBtn = element.querySelector('.download-btn');
            downloadBtn.style.display = 'none';
            
            // Wait a bit to ensure everything is rendered
            setTimeout(() => {
                html2canvas(element, {
                    scale: 2,
                    backgroundColor: '#ffffff',
                    logging: false,
                    useCORS: true,
                    allowTaint: true
                }).then(canvas => {
                    // Restore the download button
                    downloadBtn.style.display = 'flex';
                    
                    // Create download link
                    const link = document.createElement('a');
                    link.download = `gatepass-${currentGatepassId}.png`;
                    link.href = canvas.toDataURL('image/png');
                    link.click();
                    hideLoader();
                }).catch(error => {
                    console.error('Error generating image:', error);
                    // Restore the download button in case of error
                    downloadBtn.style.display = 'flex';
                    hideLoader();
                    showConfirmationDialog('Failed to download gatepass details. Please try again.');
                });
            }, 500);
        }

        // Edit gatepass
        function editGatepass(id) {
            showLoader('Loading gatepass for editing...');
            
            fetch(`${gatepassApiBaseUrl}/${id}`)
                .then(response => {
                    if (!response.ok) throw new Error('Failed to fetch gatepass');
                    return response.json();
                })
                .then(gatepass => {
                    updateMode = true;
                    console.log("newupdatedata", gatepass);
                    $('#submitGatepassBtn').html('<i class="fas fa-save me-2"></i>Update Gatepass');
                    
                    document.getElementById('gatepassId').value = gatepass.id;
                    document.getElementById('gatePassNo').value = gatepass.gatePassNo;
                    document.getElementById('date').value = gatepass.date.split('T')[0];
                    document.getElementById('companyName').value = gatepass.companyName;
                    document.getElementById('address').value = gatepass.address;
                    document.getElementById('issuedTo').value = gatepass.issuedTo;
                    document.getElementById('truckNo').value = gatepass.truckNo || '';
                    document.getElementById('driverName').value = gatepass.driverName;
                    document.getElementById('transportCompany').value = gatepass.transportCompany;
                    document.getElementById('driverPhone').value = gatepass.driverPhone;
                    document.getElementById('driverLicense').value = gatepass.driverLicense;
                    document.getElementById('remarks').value = gatepass.remarks || '';
                    
                    // Load challans for selected truck
                    loadChallansForTruck(gatepass.truckNo);
                    
                    // Open the creation section and focus on it
                    openAndFocusSection('createGatepassSection','ok');
                    hideLoader();
                })
                .catch(error => {
                    console.error('Error loading gatepass for edit:', error);
                    hideLoader();
                    showConfirmationDialog('Failed to load gatepass details.');
                });
        }

        // Open and focus on a specific section
        function openAndFocusSection(sectionId,type) {
            // Close all sections first
            if(type !== 'ok' ){
                $('.collapse').collapse('hide');
            $('.card-header').addClass('collapsed');
            
            // Open the target section
            const targetSection = document.getElementById(sectionId);
            const targetBody = targetSection.querySelector('.collapse');
            const targetHeader = targetSection.querySelector('.card-header');
            
            $(targetBody).collapse('show');
            $(targetHeader).removeClass('collapsed');
            
            // Add highlight animation
            targetSection.classList.add('highlight-section');
            
            // Scroll to the section
            targetSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
            
            // Remove highlight after animation completes
            setTimeout(() => {
                targetSection.classList.remove('highlight-section');
            }, 2000);
            }
            
        }

        // Confirm delete using Bootstrap modal
        function confirmDelete(id) {
            currentAction = () => {
                showLoader('Deleting gatepass...');
                
                fetch(`${gatepassApiBaseUrl}/${id}`, { method: 'DELETE' })
                    .then(response => {
                        if (response.ok) {
                            hideLoader();
                            showConfirmationDialog('Gatepass deleted successfully!');
                            loadGatepasses();
                        } else {
                            throw new Error('Failed to delete gatepass');
                        }
                    })
                    .catch(error => {
                        console.error('Error deleting gatepass:', error);
                        hideLoader();
                        showConfirmationDialog('Failed to delete Gatepass. Check console for details.');
                    });
            };

            showConfirmationDialog('Are you sure you want to delete this gatepass?', true);
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
                    
                    const select = document.getElementById('truckNo');
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


        // Clear form
        function clearForm() {
            document.getElementById('gatepassId').value = '';
            document.getElementById('gatePassNo').value = generategatepassNo();
            document.getElementById('date').value = '2025-10-07';
            document.getElementById('companyName').value = '';
            document.getElementById('address').value = '';
            document.getElementById('issuedTo').value = '';
            document.getElementById('truckNo').value = '';
            document.getElementById('driverName').value = '';
            document.getElementById('transportCompany').value = '';
            document.getElementById('driverPhone').value = '';
            document.getElementById('driverLicense').value = '';
            document.getElementById('remarks').value = '';
            document.getElementById('challansList').innerHTML = '<p class="text-muted">Select a truck to view associated delivery challans</p>';
            $('#submitGatepassBtn').html('<i class="fas fa-save me-2"></i>Submit Gatepass');
            updateMode = false;
        }
