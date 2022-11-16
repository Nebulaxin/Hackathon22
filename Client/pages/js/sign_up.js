function sendJSON() {
    let name = document.querySelector('#Name');
    let username = document.querySelector('#Username');
    let password = document.querySelector('#Password');
    let password2 = document.querySelector('#Password2');

    if (password.value != password2.value) {
        alert("Passwords don't match");

        return;
    }

    var xhr = new XMLHttpRequest();
    var url = "http://185.177.218.151:5050/signUp?data=" + encodeURIComponent(JSON.stringify({"username": username.value,
     "password": password.value, "name": name.value}));
    xhr.open("GET", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var json = JSON.parse(xhr.responseText);
            if (json.status == 0){
                localStorage.setItem('token', json.token);
                window.location.href = '../html/profile.html';
            }
            else {
                alert("This username already exists");
            }
        }
    };

    xhr.send();
}