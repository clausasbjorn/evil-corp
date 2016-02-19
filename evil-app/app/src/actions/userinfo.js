import { getMacInfo } from './evilhelper';
import { 
    GET_MAC_REQUEST, 
    GET_MAC_SUCCESS, 
    GET_MAC_FAILURE 
} from '../constants/ActionTypes';

function getMacRequest(){   
    return {
        type: GET_MAC_REQUEST
    }
}

function getMacSuccess(json){
    console.log("HAPPY DAYS");
    return {
        type: GET_MAC_SUCCESS,
        helperJson: json
    }
}

function getMacFailure(error){
    return {
        type: GET_MAC_FAILURE,
        error: error.toString()
    }
}

function getMac(){
    return dispatch => {
        dispatch(getMacRequest());
           
        return getMacInfo()
            .then(result => dispatch(getMacSuccess(result)))
            .catch(error => dispatch(getMacFailure(error)));
    }
}

export function fetchUserinfo(){
    return (dispatch, getState) => {
        return dispatch(getMac());
    }
}