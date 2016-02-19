import React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import { Router, useRouterHistory} from 'react-router';
import { createHashHistory } from 'history';

import createRoutes from './routes';
import configureStore from './store/configureStore';
import { fetchUserinfo, takePicutre, pictureTaken, sendUserinfo } from './actions/userinfo';

import {
    SET_NAME,
    GET_MAC_SUCCESS,
    TAKE_PICTURE,
    PICTURE_TAKEN
 } from './constants/ActionTypes';

const appHistory = useRouterHistory(createHashHistory)({queryKey: false});
const routes = createRoutes();
const store = configureStore(appHistory);

render(
    <Provider store={store}>
        <Router history={appHistory}>
            {routes}
        </Router>
    </Provider>,
    document.getElementById('root')
);

const sock = {
    proxy: null,
    signalDispatcher: () => {
        const { lastAction, userinfo } = store.getState();
        
        if(sock.proxy) {
            switch (lastAction.type) {
                case TAKE_PICTURE:
                    return sock.proxy.server.takePicture(connectionId);
                case SET_NAME:
                case GET_MAC_SUCCESS:
                case PICTURE_TAKEN:
                    return sock.proxy.server.saveIdentity(userinfo.hwaddr, userinfo.name, userinfo.pictureId);
                default:
                    return;
            }
        } else {
            return;
        }
    },
    startSignalServer: (connectionId) => {
        console.log('Starting signal server', connectionId);
        if(connectionId) {
            sock.proxy = $.connection.evilHub;
            
            sock.proxy.client.pictureTaken = (accessPointConnectionId, pictureId) => {
                console.log("EVENT PictureTaken");
                if(accessPointConnectionId == connectionId) {
                    store.dispatch(pictureTaken(pictureId))
                }
            }
            
            $.connection.hub.url = 'http://evil-signalhub.azurewebsites.net/signalr/hubs';
            $.connection.hub.start()
                .done(function () {
                    console.log('Now connected, connection ID=' + $.connection.hub.id);
                    store.dispatch(fetchUserinfo());
                })
                .fail(function (err) {
                    console.log('Could not connect to server.', err);
                });
        }
    }
}

var getUrlParameter = function getUrlParameter(sParam) {
    var sPageURL = window.location.hash.substring(3),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};

var connectionId = getUrlParameter('connectionId');
sock.startSignalServer(connectionId);
store.subscribe(() => sock.signalDispatcher());

