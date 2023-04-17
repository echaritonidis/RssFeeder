function updateContextPosition(op) {
    const element = document.querySelector(`.${op.className}`);
    element.style.left = `${op.x}px`;
    element.style.top = `${op.y}px`;
}

function bindDomChanges() {
    console.log("binded");
    document.addEventListener('click', function (event) {
        if (!event.target.classList.contains('nav-icon')) {
            DotNet.invokeMethodAsync('RssFeeder.Client', 'CloseDropDown');
        }
    });
}

function unbindDomChanges() {
    console.log("unbinded");
    document.removeEventListener('click', function (event) {
    });
}