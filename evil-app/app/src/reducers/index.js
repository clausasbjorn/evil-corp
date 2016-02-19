import { combineReducers } from 'redux';
import { routeReducer as routes } from 'react-router-redux';
import { userinfo } from './userinfo';
import { ui } from './ui';
import { lastAction } from './lastAction';

const rootReducer = combineReducers({
    userinfo,
    ui,
    routes,
    lastAction
});

export default rootReducer;