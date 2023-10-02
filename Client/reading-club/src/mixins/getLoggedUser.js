import { postAuthorized } from '../services/CrudService';

// this is mixin for options api
const getLoggedUser = {
    data(){
        return {
            getLoggedUserUrl: "api/user/getLoggedUser"
        }
    },

    methods: {
        async getLoggedUserEmail() {
            const userDetailsResponse = await postAuthorized(this.getLoggedUserUrl, {});
            let email = null;
            if (userDetailsResponse.status === true) {
                email = userDetailsResponse.data.email;
            }

            return email;
        }
    }
}

export default getLoggedUser;




// this is js module for composition api (composition api do not support mixins)

const getLoggedUserUrl = "api/user/getLoggedUser";

async function getLoggedUserEmail() {
    const userDetailsResponse = await postAuthorized(getLoggedUserUrl, {});    
    let email = null;
    if (userDetailsResponse.status === true) {
        email = userDetailsResponse.data.email;
    }

    return email;
}

export { getLoggedUserEmail };
