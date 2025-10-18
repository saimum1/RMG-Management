$(document).ready(function() {
    const apiBaseUrl = 'http://localhost:5140/api/gatepass';
    const urlParams = new URLSearchParams(window.location.search);
    const gatepassId = urlParams.get('id');

    if (!gatepassId || gatepassId === '00000000-0000-0000-0000-000000000000') {
        $('#gatepassHeaderContent').html('<p class="text-danger">Invalid or missing gatepass ID.</p>');
        return;
    }

    // Fetch gatepass data by ID
    $.getJSON(`${apiBaseUrl}/${gatepassId}`, function(gatepass) {
        // Populate header information
        $('#gatePassNo').text(`Gate Pass No: ${gatepass.gatePassNo}`);
        $('#date').text(`Date: ${new Date(gatepass.date).toLocaleDateString()}`);
        $('#companyName').text(`Company Name: ${gatepass.companyName}`);
        $('#address').text(`Address: ${gatepass.address}`);
        $('#issuedTo').text(`Issued To: ${gatepass.issuedTo}`);
        $('#remarks').text(`Remarks: ${gatepass.remarks || 'None'}`);

        // Populate details table
        const tbody = $('#detailsTable tbody');
        tbody.empty();
        gatepass.details.forEach(detail => {
            tbody.append(`
                <tr >
                    <td>${detail.slNo}</td>
                    <td>${detail.itemDescription}</td>
                    <td>${detail.quantity}</td>
                    <td>${detail.purpose}</td>
                    <td>${detail.vehicleNo || ''}</td>
                    <td>${detail.driverName || ''}</td>
                    <td>${detail.gateOutTime || ''}</td>
                    <td>${detail.authorizedBy || ''}</td>
                </tr>
            `);
        });

        // Set QR code image source
        $('#qrCode').attr('src', `${apiBaseUrl}/${gatepassId}/qr`);
    }).fail(function(jqXHR, textStatus, errorThrown) {
        console.error('Error fetching gatepass details:', textStatus, errorThrown);
        $('#gatepassHeaderContent').html('<p class="text-danger">Failed to load gatepass details.</p>');
    });
});