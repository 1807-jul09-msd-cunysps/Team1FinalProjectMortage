function loadHome() {
    $(document).ready(function () {
        $('#mortgage').slideUp(500);
        $('#home').slideDown(500);
        $('#service').slideUp(500);
        $('#apply').slideUp(500);
        $('#help').slideUp(500);

    });
}

function loadMortgage() {
    $(document).ready(function () {
        $('#mortgage').slideDown(500);
        $('#home').slideUp(500);
        $('#service').slideUp(500);
        $('#apply').slideUp(500);
        $('#help').slideUp(500);

    });
}

function loadApply() {
    $(document).ready(function () {
        $('#mortgage').slideUp(500);
        $('#home').slideUp(500);
        $('#service').slideUp(500);
        $('#apply').slideDown(500);
        $('#help').slideUp(500);

    });
}

function loadService() {
    $(document).ready(function () {
        $('#mortgage').slideUp(500);
        $('#home').slideUp(500);
        $('#service').slideDown(500);
        $('#apply').slideUp(500);
        $('#help').slideUp(500);


    });
}

function loadHelp() {
    $(document).ready(function () {
        $('#mortgage').slideUp(500);
        $('#home').slideUp(500);
        $('#service').slideUp(500);
        $('#apply').slideUp(500);
        $('#help').slideDown(500);

    });
}


function caseValidate() {
    var titl = document.getElementById("titl").value;
    var desc = document.getElementById("desc").value;
    var logi = false;
    var titb = false;
    var dscb = false;

    if (titl == "") {
        titb = false;
        $(document).ready(function () {
            $('#titlError').show(500);
        });

    } else {
        titb = true;
        $(document).ready(function () {
            $('#titlError').hide(500);
        });
    }

    if (desc == "") {
        dscb = false;
        $(document).ready(function () {
            $('#descError').show(500);
        });


    } else {
        dscb = true;
        $(document).ready(function () {
            $('#descError').hide(500);
        });

    }
}

function payment() {
    $(document).ready(function () {
        $('#rTable').slideUp(500);
        $('#makePayment').slideDown(500);
    });
}

function cancelPayment() {
    $(document).ready(function () {
        $('#makePayment').slideUp(500);
        $('#rTable').slideDown(500);
    });
}

function reason() {
    $(document).ready(function () {
        $('#lTable').slideUp(500);
        $('#reason').slideDown(500);
    });
}

function cancelReason() {
    $(document).ready(function () {
        $('#reason').slideUp(500);
        $('#lTable').slideDown(500);
    });
}

function openCase() {
    $(document).ready(function () {
        $('#caseTable').slideUp(500);
        $('#case').slideDown(500);
    });
}

function closeCase() {
    $(document).ready(function () {
        $('#caseTable').slideDown(500);
        $('#case').slideUp(500);
    });
}



//Mortgage Apply form validation


function mortDescr() {
    var mortDescr = document.getElementById("mortDescr").value;

    if (mortDescr === "") {
        $(document).ready(function () {
            $('mortDescrError').slideDown(500);
        });
        return false;
    } else {
        $(document).ready(function () {
            $('mortDescrError').slideUp(500);
        });
        return true;
    }

}



function checkAmount() {
    var amount = document.getElementById("applicationAmount").value;


    if (Number(amount) < 0) {
        document.getElementById("applicationAmount").value = 0;
    }
}

function checkSSN() {
    var SSN = document.getElementById("SSN").value;

    var ssnPattern = /^[0-9]{3}\-?[0-9]{2}\-?[0-9]{4}$/;
    if (!ssnPattern.test(SSN)) {
        $(document).ready(function () {
            $('#errorSSN').slideDown(500);
        });
        document.getElementById("SSN").value = "";
        return false;
    } else {
        $(document).ready(function () {
            $('#errorSSN').slideUp(500);
        });
        return true;
    }

}

function checkTitle() {

    var title = document.getElementById("mortgageTitle").value;

    if (title === "") {
        $(document).ready(function () {
            $('#errorMT').slideDown(500);
        });
        return false;
    } else {
        $(document).ready(function () {
            $('#errorMT').slideUp(500);
        });
        return true;
    }
}

function checkAddress() {
    var fname = document.getElementById("first-name").value;
    var lname = document.getElementById("last-name").value;
    var line1 = document.getElementById("address-line1").value;
    var city = document.getElementById("city").value;
    var regin = document.getElementById("region").value;
    var zip = document.getElementById("postal-code").value;

    if (fname === "" || lname === "" || line1 === "" || city === "" || regin === "" || zip === "") {
        $(document).ready(function () {
            $('.addressError').slideDown(500);
        });
        return false;
    } else {
        $(document).ready(function () {
            $('.addressError').slideUp(500);
        });
        return true;
    }

}


function applyValidate() {
    mortDescr();
    checkAmount();
    checkSSN();
    checkTitle();
    checkAddress();
    if (mortDescr() & checkSSN() & checkTitle() & checkAddress()) {
        {

            //checks if all conditions are evaluating true
            //alert(String(mortDescr()) + " " + String(checkSSN()) + " " + String(checkTitle()) + " " + String(checkAddress()));


            var Application = {
                "MortgageApplication": [

                    {
                        "ssn": document.getElementById("SSN").value,
                        "currency": document.getElementById("currency").value,
                        "mortgageTitle": document.getElementById("mortgageTitle").value,
                        "description": document.getElementById("mortDescr").value,
                        "amount": document.getElementById("applicationAmount").value,
                        "firstName": document.getElementById("first-name").value,
                        "lastName": document.getElementById("last-name").value,
                        "address1": document.getElementById("address-line1").value,
                        "address2": document.getElementById("address-line2").value,
                        "cityOrTown": document.getElementById("city").value,
                        "stateOrProvince": document.getElementById("region").value,
                        "zip": document.getElementById("postal-code").value,
                        "country": document.getElementById("country").value
                    }
                ]
            };
            console.log(Application);


            $.ajax({

                type: 'POST',

                url: 'http://mortage.azurewebsites.net/api/application',

                data: JSON.stringify(Application),

                contentType: "application/json",

                dataType: 'JSON',

                success: function () {

                    console.log("Success");

                },

                Error: function () {

                    console.log("ERROR");

                }

            });


        }
    }

}