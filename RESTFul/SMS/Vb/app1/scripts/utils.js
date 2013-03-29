var sPH = supportsPlaceholder();

function setup(){
  //BEGIN FORM ELEMENTS
  var address = "Address";
  var msgID = "Message ID";

  var addressBox = contentForm.elements["address"]
  var	msgBox = contentForm.elements["MessageId"];
  //END FORM ELEMENTS

  //only setup listeners if it doesn't support html5 feature
  if(!sPH){
    //setup listeners for selecting box
    addressBox.setAttribute('onfocus', 'boxSelected(addressBox)');
    msgBox.setAttribute('onfocus', 'boxSelected(msgBox)');
    
    partBox.setAttribute('onfocus', 'boxSelected(partBox)');

    //setup listeners for de-selecting box
    addressBox.setAttribute('onblur', 'boxUnselected(addressBox)');
    msgBox.setAttribute('onblur', 'boxUnselected(msgBox)');
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
