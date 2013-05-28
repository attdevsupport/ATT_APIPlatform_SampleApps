var sPH = supportsPlaceholder();

var address = "Address";
var msgID = "Message ID";

var sendForm;
var idForm;
var addressBox;
var msgBox;

var prevValue = "";


function setup(){
  //BEGIN FORM ELEMENTS
  sendForm = document.getElementById("sendSMSForm");
  idForm = document.getElementById("getStatusForm");

  addressBox = sendForm.elements["address"]
  msgBox = idForm.elements["messageId"];
  //END FORM ELEMENTS

  //only setup listeners if it doesn't support html5 feature
  if(!sPH){
    //setup listeners for selecting box
    addressBox.setAttribute('onfocus', 'boxSelected(this)');
    msgBox.setAttribute('onfocus', 'boxSelected(this)');
    
    //setup listeners for de-selecting box
    addressBox.setAttribute('onblur', 'boxUnselected(this)');
    msgBox.setAttribute('onblur', 'boxUnselected(this)');
    setDefaults();
  }
}

function setDefaults(){
  //set default values into box
  if(!sPH){
    if (addressBox.value == "")
      addressBox.value = address;

    if (msgBox.value == "")
      msgBox.value = msgID;
  }
}

function boxSelected(textBox){
  if (!sPH){
    if (textBox.value == msgID || textBox.value == address) {
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
