import React, { Component, PropTypes } from 'react';
import { Link } from 'react-router';

class EvilForm extends Component{
    constructor(props) {
        super(props)
    }
    
    render(){
        let { value, onChange, ...other } = this.props;
        
        return(
            <input type="text" placeholder="Hva heter du?" value={value} onChange={e => onChange(e.target.value)} />
        );
    }
}

EvilForm.propTypes = {
  value: PropTypes.string.isRequired,
  onChange: PropTypes.func.isRequired
};

export default EvilForm;