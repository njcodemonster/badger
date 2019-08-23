﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/claimHub").build();

//Disable send button until connection is established
ToggleClaim(true);

connection.on("PushClaim", function (claim) {
    var po_id = $("#poId").val();
    if (claim.po_id == po_id || po_id == undefined) {
        BindInspectClaimHtml(claim);
        BindPublishClaimHtml(claim);
    }
});

connection.start().then(function () {
   // document.getElementById("btnClaim").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

function BindInspectClaimHtml(claim) {
    var title = "<b>Inspect</b><br />";
    var claimButton = title + '<button type="button" class="btn btn-link btn-sm" id="claim-inspect-btn" onclick="Claim(undefined,' + claimerType.InspectClaimer + ')">Claim</button>';
    if (claim.inspect_claimer > 0) {
        claimButton = title + '<button type="button" class="btn btn-danger btn-sm" onclick="RemoveClaim(undefined,' + claimerType.InspectClaimer + ')" id="remove-claim-inspect-btn">' +
            ' <span style = "padding:inherit">' + claim.inspect_claimer_name + '</span>' +
            ' <span class="fa fa-times"></span></button >';
        $("#inspect-claim").remove("#claim-inspect-btn");
    }
    $("#inspect-claim").remove("#remove-claim-inspect-btn");
    try {
        claimButton = claimButton.replace("undefined", claim.po_id);
        document.getElementById('inspect-claim-' + claim.po_id).innerHTML = claimButton;
    } catch (e) {
        document.getElementById('inspect-claim').innerHTML = claimButton;
    }
    
}

function BindPublishClaimHtml(claim) {
    var title = "<b>Go Live</b><br />";
    var claimButton = title + '<button type="button" class="btn btn-link btn-sm claim" id="claim-publsih-btn" onclick="Claim(undefined,' + claimerType.PublishClaimer + ')">Claim</button>';
    if (claim.publish_claimer > 0) {
        claimButton = title + '<button type="button" class="btn btn-danger btn-sm remove-claim" onclick="RemoveClaim(undefined,' + claimerType.PublishClaimer + ')" id="remove-claim-publish-btn">' +
            ' <span style = "padding:inherit">' + claim.publish_claimer_name + '</span>' +
            ' <span class="fa fa-times"></span></button >';
        $("#publish-claim").remove("#claim-publish-btn");
    }
    $("#publish-claim").remove("#remove-claim-publish-btn");
    try {
        claimButton = claimButton.replace("undefined", claim.po_id);
        document.getElementById('publish-claim-' + claim.po_id).innerHTML = claimButton;
    } catch (e) {
        document.getElementById('publish-claim').innerHTML = claimButton;
    }

}

//document.getElementById("btnClaimInspect").addEventListener("click", function (event) {
//    var poId = document.getElementById("poid").value;
//    var userId = document.getElementById("userid").value;
//    var userName = document.getElementById("username").value;
//    connection.invoke("SendClaim", { POId: poId, UserId: userId, UserName: userName }).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});

function Claim(po_id, claimerType) {
    var poId = po_id < 1 ? $("#poId").val() : po_id;
    $.ajax({
        url: '/purchaseorders/claim/',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ po_id: poId, claim_type: claimerType }),
        beforeSend: function () {
            ToggleClaim(true);
        },
        success: function (response) {
            PushNotifyClaim(response);
            ToggleClaim(false);
        },
        error: function (ex) {
            alert("error: updating claim");
            console.log(ex);
        },
        complete: function (data) {

        }
    });
}

function RemoveClaim(po_id, claimerType) {
    var poId = po_id < 1 ? $("#poId").val() : po_id;
    $.ajax({
        url: '/purchaseorders/removeclaim/',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ po_id: poId, claim_type: claimerType }),
        beforeSend: function () {
            ToggleClaim(true);
        },
        success: function (response) {
            PushNotifyClaim(response);
            ToggleClaim(false);
        },
        error: function (ex) {
            alert("error: removing claim");
            console.log(ex);
        }
    });
}

function LoadClaim() {
    var poId = $("#poId").val();
    $.ajax({
        url: '/purchaseorders/loadclaim/' + poId,
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json',
        success: function (response) {
            console.log(response);
            BindInspectClaimHtml(response);
            BindPublishClaimHtml(response);
        },
        error: function (ex) {
            alert("error: loading claim");
            console.log(ex);
        }
    });
}

function ToggleClaim(disabled) {
    document.getElementsByClassName("claim").disabled = disabled;
    document.getElementsByClassName("remove-claim").disabled = disabled;
}

function PushNotifyClaim(claim) {
    connection.invoke("SendClaim", claim).catch(function (err) {
        return console.error(err.toString());
    });
}