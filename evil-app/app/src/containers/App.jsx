import React, { Component, PropTypes } from 'react';
import { connect } from 'react-redux';
import { Router, Route } from 'react-router';
import { EvilForm } from '../components';

class App extends Component{
    render(){
        let { title, children, ...other } = this.props;
        
        return (
            <div>
            {title}    
            <EvilForm></EvilForm>   
            </div>
        );
    } 
}

App.propTypes = {
    title: PropTypes.string
};

// Which props do we want to inject, given the global state?
// Note: use https://github.com/faassen/reselect for better performance.
function select(state) {
    const { name } = state.ui;
    return {
        title: name
    };
}

export default connect(select)(App);