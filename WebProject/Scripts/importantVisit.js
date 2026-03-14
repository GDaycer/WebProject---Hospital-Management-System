function loadImportantVisits(url) {

    $.ajax({
        url: url,  //go to this url
        type: 'GET', //fetch the data
        success: function (data) {
            $('#patient_stats_extended').html(data);
        }
    });
}

function closeImportantVisits(url) {
    $.ajax({
        url: url,
        type: 'GET',
        success: function (data) {
            $('#patient_stats_extended').html(data);
        }
    });
}