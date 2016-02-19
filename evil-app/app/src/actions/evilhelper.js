export function getMacInfo(){
    return fetch('http://10.59.9.60')
    .then(function(response) {
        if (response.status == 200) {
            return response.json();
        }
    })
    .then(function(json) {
        console.log(json);
        return json;
    });
}