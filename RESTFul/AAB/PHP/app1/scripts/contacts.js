function addFields(fieldName, fields, limit, types, table_id){
    var table = document.getElementById(table_id);
    var fieldsNames = fields.split(" ");
    var fieldTypes = types.split(" ");
    var rowCount = table.rows.length;
    var row = table.insertRow(rowCount);
    if(rowCount < limit){
        var cell0 = row.insertCell(0);
        var cell1 = row.insertCell(1);
        var labelPref = document.createElement("label");
        var elementPref = document.createElement("input");
        elementPref.type = "radio";
        elementPref.name=fieldName + "Pref";
        elementPref.value=rowCount; 
        cell0.appendChild(elementPref);
        labelPref.innerHTML="PREF";
        cell0.appendChild(labelPref);
        for(var i = 0; i < fieldsNames.length; i++){
            var element = document.createElement("input");
            element.type = "text";
            element.id=fieldName + "_" + fieldsNames[i]+rowCount;
            element.name=fieldName+"[]["+fieldsNames[i]+"]";
            element.placeholder=fieldsNames[i];
            cell1.appendChild(element);
        }
        var elementSel = document.createElement("select");
        elementSel.setAttribute("id", fieldName+"Type"+rowCount);
        elementSel.setAttribute("name", fieldName+"[][type]");
        for(var j = 0; j < fieldTypes.length; j++){
            var option_sel = document.createElement("option");
            option_sel.setAttribute("value", fieldTypes[j]);
            option_sel.innerHTML=fieldTypes[j];
            elementSel.appendChild(option_sel);
        }
        cell1.appendChild(elementSel);
    }else{
        alert("Exceeded the limit");
    }
}

function showWindows(rad){
    var rads=document.getElementsByName(rad.name);
    document.getElementById('createContact').style.display=(rads[0].checked)?'block':'none' ;
    document.getElementById('updateContact').style.display=(rads[1].checked)?'block':'none' ;
    document.getElementById('getContacts').style.display=(rads[2].checked)?'block':'none' ;
    document.getElementById('updateMyinfo').style.display=(rads[3].checked)?'block':'none' ;
    document.getElementById('createGroup').style.display=(rads[4].checked)?'block':'none' ;
    document.getElementById('updateGroup').style.display=(rads[5].checked)?'block':'none' ;
    document.getElementById('deleteGroup').style.display=(rads[6].checked)?'block':'none' ;
    document.getElementById('getGroups').style.display=(rads[7].checked)?'block':'none' ;
    document.getElementById('getGroupContacts').style.display=(rads[8].checked)?'block':'none' ;
}
