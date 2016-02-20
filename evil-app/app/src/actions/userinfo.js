import { getMacInfo } from './evilhelper';
import { 
    GET_MAC_REQUEST, 
    GET_MAC_SUCCESS, 
    GET_MAC_FAILURE,
    SET_NAME,
    TAKE_PICTURE,
    PICTURE_TAKEN
} from '../constants/ActionTypes';

function getMacRequest(){   
    return {
        type: GET_MAC_REQUEST
    }
}

function getMacSuccess(json){
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

export function setName(name) {
    return {
        type: SET_NAME,
        name: name
    }    
}

export function takePicture(){   
    return {
        type: TAKE_PICTURE
    }
}

export function pictureTaken(pictureId){   
    return {
        type: PICTURE_TAKEN,
        pictureId: pictureId
    }
}

export function fetchUserinfo(){
    return (dispatch, getState) => {
        return dispatch(getMac());
    }
}