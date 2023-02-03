var time_handler = setInterval(() => {
    const element = document.querySelector(`[class*='sign-wrapper']`);
    if (!element)
        return;

    let event = new MouseEvent("click", {
        bubbles: true,
        cancelable: true,
        view: window
    });
    element.dispatchEvent(event);
    clearInterval(time_handler);
}, 1000);