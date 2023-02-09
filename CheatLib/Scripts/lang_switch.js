var lang = '{0}';
var cookieKeys = ['mi18nLang', 'locale'];

var cookies = document.cookie.split(';');
var changed = false;
var result = [];
for (let cookie of cookies) {
    let [k, v] = cookie.trim().split('=');

    for (let cookieKey of cookieKeys) {
        if (k == cookieKey && v != lang) {
            v = lang;
            changed = true;
        }
        result.push(k + '=' + v);
    }
}

if (changed) {
    document.cookie = result.join(';');
    setTimeout(() => {
        location.reload();
    }, 50);
}



