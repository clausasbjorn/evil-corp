import React, { Component, PropTypes } from 'react';
import { Link } from 'react-router';

class EvilForm extends Component{
    render(){
        let { name, ...other } = this.props;
        
        return(
            <input
                type="text"
                value={name}
              
            />
        );
        
        //  onChange={this.handleChange}
    }
}

EvilForm.propTypes = {
    name: PropTypes.string
};

export default EvilForm;