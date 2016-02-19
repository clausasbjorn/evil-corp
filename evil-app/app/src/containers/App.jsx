import React, { Component, PropTypes } from 'react';
import { connect } from 'react-redux';
import { Router, Route } from 'react-router';
import { EvilForm } from '../components';
import { setName } from '../actions/userinfo'

class App extends Component{
      constructor(props) {
    super(props)
    this.handleChange = this.handleChange.bind(this)
  }
  
    render(){
        let { userName } = this.props;
        
        return (
            <EvilForm value={userName} onChange={this.handleChange}></EvilForm>   
        );
    } 
    
    handleChange(text) {
        this.props.dispatch(setName(text));
    }
}

App.propTypes = {
    title: PropTypes.string,
        dispatch: PropTypes.func.isRequired
};

// Which props do we want to inject, given the global state?
// Note: use https://github.com/faassen/reselect for better performance.
function select(state) {
    const { name } = state.userinfo;
    return {
        userName: name
    };
}

export default connect(select)(App);