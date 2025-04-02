import http from 'k6/http';
import { sleep, check } from 'k6';

export let options = {
    stages: [
        { duration: '10s', target: 5 },   
        { duration: '20s', target: 20 },  
        { duration: '10s', target: 0 }    
    ],
    thresholds: {
        'http_req_duration': ['p(95)<500'], 
        'http_req_failed': ['rate<0.01'],  
    }
};

export default function () {
    let res = http.get('http://localhost:5149/'); 
    check(res, {
        'status is 200': (r) => r.status === 200,
        'response time < 500ms': (r) => r.timings.duration < 500
    });
    sleep(1);
}

