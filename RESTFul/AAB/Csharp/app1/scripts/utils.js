var sPH = supportsPlaceholder();

var resultsTable = document.getElementById("kvp");

function setup(){
    //BEGIN FORM ELEMENTS
    var msgID = "Message ID";
    var partNumber = "Part Number";
    var headerCount = "Header Count";
    var indexCursor = "Index Cursor";

    var headerForm = document.getElementById("msgHeaderForm");
    var contentForm = document.getElementById("msgContentForm");

    var headerBox = headerForm.elements["headerCountTextBox"];
    var	indexBox = headerForm.elements["indexCursorTextBox"];
    var msgBox = contentForm.elements["MessageId"];
    var partBox = contentForm.elements["PartNumber"];
    //END FORM ELEMENTS

    //Setup misc variables
    var prevValue = "";
    var resultsTable = document.getElementById("kvp");

    //only setup listeners if it doesn't support html5 feature
    if(!sPH){
        //setup listeners for selecting box
        headerBox.setAttribute('onfocus', 'boxSelected(headerBox)');
        indexBox.setAttribute('onfocus', 'boxSelected(indexBox)');
        msgBox.setAttribute('onfocus', 'boxSelected(msgBox)');
        partBox.setAttribute('onfocus', 'boxSelected(partBox)');

        //setup listeners for de-selecting box
        headerBox.setAttribute('onblur', 'boxUnselected(headerBox)');
        indexBox.setAttribute('onblur', 'boxUnselected(indexBox)');
        msgBox.setAttribute('onblur', 'boxUnselected(msgBox)');
        partBox.setAttribute('onblur', 'boxUnselected(partBox)');
    }
}

function setDefaults(){
    //set default values into box
    if(!sPH){
        if (headerBox.value == "")
            headerBox.value = headerCount;
        if (indexBox.value == "")
            indexBox.value = indexCursor;

        if (msgBox.value == "")
            msgBox.value = msgID;
        if (partBox.value == "")
            partBox.value = partNumber;
    }
}

function boxSelected(textBox){
    if (!sPH){
        if (textBox.value == msgID || textBox.value == partNumber || textBox.value == headerCount || textBox.value == indexCursor){
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

function chooseSelect(r,s){
    var msgBox = document.getElementById("MessageId");
    var partBox = document.getElementById("PartNumber");
    var button = document.getElementById("getMessageContent");
    var row = document.getElementById(r);

    var pNum = s.options[s.selectedIndex].innerHTML.trim()[0];
    var msgId = row.cells[0].innerHTML.trim();

    msgBox.value = msgId;
    partBox.value = pNum;

    //msgBox.scrollIntoView();
    s.selectedIndex = 0;
    button.click();
}

function toggle(showHideDiv, switchTextDiv, title) {
    var ele = document.getElementById(showHideDiv);
    var text = document.getElementById(switchTextDiv);
    if (ele.style.display == "block") {
        ele.style.display = "none";
        text.innerHTML = "";
        text.innerHTML = " " + title;
    }
    else {
        ele.style.display = "block";
        text.innerHTML = "Hide " + title;
    }
} 
