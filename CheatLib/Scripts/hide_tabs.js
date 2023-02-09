var g_c_classes = ['UpdateHandler_update__vvunN', 'UpdateHandler_updateContainer__CvwAU'];

var g_c_timer = setInterval(() => {
    for (let i = g_c_classes.length - 1; i >= 0; i--) {
        var clazz = g_c_classes[i];
        var elem = document.getElementsByClassName(clazz);
        if (!elem)
            continue;

        elem.style.visibility = 'collapse';
        g_c_classes = g_c_classes.filter((value, index) => index != i);
    }
    if (!g_c_classes.length) {
        clearInterval(g_c_timer);
    }

}, 200);