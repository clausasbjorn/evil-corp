import request from 'request';
export function getMacInfo(){
    return new Promise((resolve, reject) => {
        return request.get('http://10.59.9.60');
    });
}