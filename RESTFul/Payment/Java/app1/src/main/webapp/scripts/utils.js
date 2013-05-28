
function toggle(showHideDiv, switchTextDiv, title) {
	var ele = document.getElementById(showHideDiv);
	var text = document.getElementById(switchTextDiv);
	if(ele.style.display == "block") {
    		ele.style.display = "none";
		text.innerHTML = "show";
		text.innerHTML = "Show " + title;
  	}
	else {
		ele.style.display = "block";
		text.innerHTML = "Hide " + title;
	}
} 