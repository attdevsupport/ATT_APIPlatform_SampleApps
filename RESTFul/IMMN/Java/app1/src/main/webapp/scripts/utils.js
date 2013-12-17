
function toggle(div) {
  var ele = document.getElementById(div);
  var text = document.getElementById(div+"Toggle");
  if(ele.style.display == "block") {
    ele.style.display = "none";
    tt = text.innerHTML.split(":");
    title = tt[tt.length-1];
    text.innerHTML = "Show: " + title;
  }
  else {
    ele.style.display = "block";
    tt = text.innerHTML.split(":");
    title = tt[tt.length-1];
    text.innerHTML = "Hide: " + title;
  }
} 
