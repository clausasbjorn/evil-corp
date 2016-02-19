import React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import { Router, useRouterHistory} from 'react-router';
import { createHashHistory } from 'history';

import createRoutes from './routes';
import configureStore from './store/configureStore';
import { fetchUserinfo } from './actions/userinfo';

const appHistory = useRouterHistory(createHashHistory)({queryKey: false});
const routes = createRoutes();
const store = configureStore(appHistory);

store.dispatch(fetchUserinfo());

render(
    <Provider store={store}>
        <Router history={appHistory}>
            {routes}
        </Router>
    </Provider>,
    document.getElementById('root')
);