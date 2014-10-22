function hideUnless(select, element, value) {
    if (typeof select === 'string')
        select = document.getElementById(select);

    var i = select.selectedIndex;
    var context = select[i].text;

    var e = document.getElementById(element);

    if (context === value)
        e.hidden = false;
    else
        e.hidden = true;
}
