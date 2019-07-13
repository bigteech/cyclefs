import React from 'react';


export default function(f1, f2, f3, f4) {
    class Temp extends React.Component {
        constructor(props) {
            super(props);
            this.state= {};
            f4([props, (function(p) {
                this.state[p[0]] = p[1];
            }).bind(this)]);
            f1((function(p) {
                this.state[p[0]] = p[1];
                this.setState(this.state);
            }).bind(this))
        }

        unMounted() {
            f2();
        }

        render() {
            return f3(this.state);
        }
    }
    return function(p) {
        return function(k) {
            return React.createElement(Temp, p, k)
        }
    }
}