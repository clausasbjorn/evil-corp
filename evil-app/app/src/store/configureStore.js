import { createStore, applyMiddleware } from 'redux';
import thunk from 'redux-thunk';
import createLogger from 'redux-logger';
import { syncHistory } from 'react-router-redux';
import initialState from './initialState';
import rootReducer from '../reducers';

export default function configureStore(history){
    const router = syncHistory(history);
    const logger = createLogger();
    
    let middleware = [thunk, router, logger];
    
    const finalCreateStore = applyMiddleware(...middleware)(createStore);
    const store = finalCreateStore(rootReducer, initialState);
    
    return store;
}