//Функция отображения PopUp
function PopUpShow(duration) {
    $("#popup1").show();
    setTimeout(function () { PopUpHide(); }, duration);
}
//Функция скрытия PopUp
function PopUpHide() {
    $("#popup1").hide();
}
