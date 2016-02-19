import {
    GET_MAC_REQUEST,
    GET_MAC_SUCCESS,
    GET_MAC_FAILURE,
    SET_NAME
 } from '../constants/ActionTypes';

//const defaultState = { items: { subscriptions: [], receivers: [] }};
export function userinfo(state = {}, action){
    switch(action.type){
        case GET_MAC_REQUEST:
            return state;
        case GET_MAC_SUCCESS:
            return Object.assign({}, state, {
                hwaddr: action.helperJson.hwaddr,
                ip: action.helperJson.ip
            });
        case SET_NAME:
            return Object.assign({}, state, {
                name: action.name
            });
        default:
            return state;
    }
}