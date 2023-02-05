var classes = ['UpdateHandler_update__vvunN', 'UpdateHandler_updateContainer__CvwAU'];

var timer = setInterval(() => {
    for (let i = classes.length - 1; i >= 0; i--) {
        var clazz = classes[i];
        var elem = document.getElementsByClassName(clazz);
        if (!elem)
            continue;

        elem.style.visibility = 'collapse';
        classes = classes.filter((value, index) => index != i);
    }
    if (!classes.length) {
        clearInterval(timer);
    }

}, 200);