import { combineReducers } from 'redux';
import { routeReducer as routes } from 'react-router-redux';
import { userinfo } from './userinfo';
import { ui } from './ui';

const rootReducer = combineReducers({
    userinfo,
    ui,
    routes
});

export default rootReducer;