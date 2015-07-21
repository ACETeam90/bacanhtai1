// Check lại Radio (>= 2 lựa chọn) theo giá trị của input
function CheckInputRadioByValue() {
    var arrInput = listInputRadio.split(',');
    for (var i = 0; i < arrInput.length; i++) {
        var number = $("#" + arrInput[i]).val(); // lấy giá trị
        $("#rd_value_" + arrInput[i] + "_" + number).attr("checked", true);
    }
}

// Disable các radio trên view Details và Check
function DisableInputRadio() {

}

// Enable các trường dữ liệu cần nhập lại, dùng cho loại > 2 đáp án
function EnableInputRadio() {
    var strTruongNhapLai = $("#TruongNhapLai").val();
    var arrInput_ID = strTruongNhapLai.split(',');
    for (var i = 0; i < arrInput_ID.length - 1; i++) {
        if (listInputRadio.indexOf(arrInput_ID[i] + ",") != -1) {
            $("input[name='rd_value_" + arrInput_ID[i] + "']").attr("disabled", false); // Trường hợp là Input là radio , để Disabled
        }
        else {
            $("input[name='" + arrInput_ID[i] + "']").attr("readonly", false); // Trường hợp Input là TextBox, để Readonly vì Disabled, textbox không nhận giá trị
        }
        $("label[for='" + arrInput_ID[i] + "']").css('color', 'red'); // Bôi màu đỏ label của các trường dữ liệu sai (cần nhập lại)
    }
}

// ******* Dùng cho View Check (Xóa dữ liệu Trường nhập lại)  ***********
// Chỉnh css html cho các trường dữ liệu đã sửa
// Đặt dưới cùng vì hàm này set lại trường nhập lại = null
function SetIncorectHtml() {
    var strTruongNhapLai = $("#TruongNhapLai").val();
    if (strTruongNhapLai != "") {
        var arrInput_ID = strTruongNhapLai.split(',');
        var arrLabel = "";
        for (var i = 0; i < arrInput_ID.length - 1; i++) {
            //var label = $('label[for="' + arrInput_ID[i] + '"]').html();
            //arrLabel += label + ",";
            $("#" + arrInput_ID[i]).css('color', 'red');

            // Nếu là NumericTextBox
            if ($("#" + arrInput_ID[i]).hasClass("S4T_NumericTextBox") == true) {
                $("#" + arrInput_ID[i]).kendoNumericTextBox({
                    format: "n0",
                    spinners: false
                });
            } else if ($("#" + arrInput_ID[i]).hasClass("S4T_AutoComplete") == true) {
                $("#" + arrInput_ID[i]).kendoAutoComplete();
            }
            $("label[for='" + arrInput_ID[i] + "']").css('color', 'red');
        }
        //$("#txtTruongNhapLai").val(arrLabel);
    }
    // Set gia tri ve null
    $("#TruongNhapLai").val("");
    $("#txtTruongNhapLai").val(""); // Khi mở View Check, nội dung trường nhập lại sẽ xóa để tránh duplicate dữ liệu
}

// ******* Dùng cho View Edit (Không xóa dữ liệu Trường nhập lại)  ***********
// Chỉnh css html cho các trường dữ liệu đã sửa
function GetLabelInputIncorrectHtml() {
    var strTruongNhapLai = $("#TruongNhapLai").val();
    if (strTruongNhapLai != "") {
        var arrInput_ID = strTruongNhapLai.split(',');
        var arrLabel = "";
        for (var i = 0; i < arrInput_ID.length - 1; i++) {
            var label = $('label[for="' + arrInput_ID[i] + '"]').html();
            arrLabel += label + ",";
            $("#" + arrInput_ID[i]).css('color', 'red');
            
            // Nếu là NumericTextBox
            if ($("#" + arrInput_ID[i]).hasClass("S4T_NumericTextBox") == true) {
                $("#" + arrInput_ID[i]).kendoNumericTextBox({
                    format: "n0",
                    spinners: false
                });
            } else if ($("#" + arrInput_ID[i]).hasClass("S4T_AutoComplete") == true) {
                $("#" + arrInput_ID[i]).kendoAutoComplete();
            }
            $("label[for='" + arrInput_ID[i] + "']").css('color', 'red');
        }
        $("#txtTruongNhapLai").val(arrLabel);
    }
}

// Đóng/ mở fiedset lịch nhập lại báo cáo
function OpenDatePopup() {
    if ($("#fsLichNhapLieu").css('display') == 'none') {
        $("#fsLichNhapLieu").show();
        $("#txtTuNgay").focus();
        var aTag = $("#txtTuNgay");
        $('html,body').animate({ scrollTop: aTag.offset().top }, 'slow');
    } else {
        $("#fsLichNhapLieu").hide();
    }
}

// không dùng nữa
// Check lại radio Yes/No theo giá trị của input
//function CheckRadioByInputValue() {
//    var arrInput = listInputRadio.split(',');
//    for (var i = 0; i < arrInput.length; i++) {
//        if ($("#" + arrInput[i]).val() == 1) {
//            $("#rd_value_" + arrInput[i] + "_Yes").attr("checked", true);
//        } else if ($("#" + arrInput[i]).val() == 0) {
//            $("#rd_value_" + arrInput[i] + "_No").attr("checked", true);
//        }
//    }
//}

// không dùng nữa
// Enable các trường dữ liệu cần nhập lại
//function EnableInput() {
//    var strTruongNhapLai = $("#TruongNhapLai").val();
//    var arrInput_ID = strTruongNhapLai.split(',');
//    for (var i = 0; i < arrInput_ID.length - 1; i++) {
//        // Trường hợp là input là radio
//        if (listInputRadio.indexOf(arrInput_ID[i]) != -1) {
//            $("#rd_value_" + arrInput_ID[i] + "_No").attr("disabled", false);
//            $("#rd_value_" + arrInput_ID[i] + "_Yes").attr("disabled", false);
//        } else {
//            $("#" + arrInput_ID[i]).attr("readonly", false);
//            $("#" + arrInput_ID[i]).css('color', 'red');
//        }
//        $("label[for='" + arrInput_ID[i] + "']").css('color', 'red');
//    }
//}
