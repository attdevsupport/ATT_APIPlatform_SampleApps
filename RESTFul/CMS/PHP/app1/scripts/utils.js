var sPH = supportsPlaceholder();

var address = "Address";
var number = "Number";
var message = "Message";

var prevValue = ""

var contentForm;
             
var addressBox;
var numberBox;
var msgBox; 

function setup(){
    //BEGIN FORM ELEMENTS

    contentForm = document.getElementById("msgContentForm");
    var elements = contentForm.elements;

    addressBox = elements["txtNumberToDial"];
    numberBox = elements["txtNumber"];
    msgBox = elements["txtMessageToPlay"];
    //END FORM ELEMENTS

    //only setup listeners if it doesn't support html5 feature
    if(!sPH){
        //setup listeners for selecting box
        addressBox.setAttribute('onfocus', 'boxSelected(this)');
        msgBox.setAttribute('onfocus', 'boxSelected(this)');
        numberBox.setAttribute('onfocus', 'boxSelected(this)');

        //setup listeners for de-selecting box
        addressBox.setAttribute('onblur', 'boxUnselected(this)');
        msgBox.setAttribute('onblur', 'boxUnselected(this)');
        numberBox.setAttribute('onblur', 'boxUnselected(this)');
        setDefaults();
    }
}

function setDefaults(){
    //set default values into box
    if(!sPH){
        if (addressBox.value == "")
            addressBox.value = address;

        if (msgBox.value == "")
            msgBox.value = message;

        if (numberBox.value == "")
            numberBox.value = number;
    }
}

function boxSelected(textBox){
    if (!sPH){
        if (textBox.value == number || textBox.value == address || textBox.value == message) {
            prevValue = textBox.value;
            textBox.value = "";
        }
    }
}

function boxUnselected(textBox){
    if (!sPH){
        if (textBox.value == "")
            textBox.value = prevValue;
    }
}

function supportsPlaceholder() {
    var i = document.createElement('input');
    return 'placeholder' in i;
}
