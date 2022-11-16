function sendJSON() {

    let sign_in = document.querySelector('#SignIn');
    let sign_up = document.querySelector('#SignUp');
    
    var xhr = new XMLHttpRequest();
    var url = "http://185.177.218.151:5050/getProfile?data=" + encodeURIComponent(JSON.stringify({"token": localStorage.getItem('token')}));
    xhr.open("GET", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var json = JSON.parse(xhr.responseText);
            if (json.status == 0){
                sign_in.innerHTML = json.username;
                sign_in.setAttribute("href", "/Client/pages/html/profile.html") 
                sign_up.innerHTML = "Log Out"
                sign_up.setAttribute("onclick", "LogOut()");
                sign_up.setAttribute("href", "")
            }

        }
    };
    xhr.send();
}

function LogOut() {
    localStorage.removeItem('token');
    location.reload();
}