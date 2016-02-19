import {
    GET_MAC_REQUEST,
    GET_MAC_SUCCESS,
    GET_MAC_FAILURE
 } from '../constants/ActionTypes';

//const defaultState = { items: { subscriptions: [], receivers: [] }};
export function userinfo(state = {}, action){
    switch(action.type){
        case GET_MAC_REQUEST:
            return state;
        case GET_MAC_SUCCESS:{
            return Object.assign({}, state, {
                items: action.items
            });
        }
        default:
            return state;
    }
}