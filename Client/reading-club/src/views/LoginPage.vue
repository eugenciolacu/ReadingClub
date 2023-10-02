<template>
    <div class="container-sm">
        <form>
            <div class="mb-3">
                <label for="emailInput" class="form-label">Email</label>
                <input v-model="email" type="email" class="form-control" id="emailInput" placeholder="Email">
                <div v-if="formValidationMsgs.email != ''" class="form-text">
                    <span v-html="formValidationMsgs.email"></span>
                </div>
            </div>

            <div class="mb-3">
                <label for="passwordInput" class="form-label">Password</label>
                <input v-model="password" :type="showPassword ? 'text' : 'password'" class="form-control" id="passwordInput" placeholder="Password">
                <div v-if="formValidationMsgs.password != ''" class="form-text">
                    <span v-html="formValidationMsgs.password"></span>
                </div>
            </div>

            <div v-if="formValidationMsgs.failLogin != ''" class="form-text">
                {{ formValidationMsgs.failLogin }}
            </div>

            <div class="mb-3 form-check">
                <input v-model="showPassword" type="checkbox" class="form-check-input" id="showPasswordInput">
                <label class="form-check-label" for="showPasswordInput"> Show password </label>
            </div>

            <div>
                <button @click.prevent="onLoginButtonClick" type="submit" class="btn btn-dark mb-3 container">Login</button>
            </div>

            <div>
                <button @:click.prevent="onRegisterButtonClick" class="btn btn-dark mb-3 container">Register</button>
            </div>
        </form>
    </div>
</template>
    
<script>
import router from '@/router/index.js';
import { postAnonymous } from '@/services/CrudService';
import { setToken } from '@/services/AuthenticationService';

export default {
    name: 'LoginPage',

    data() {
        return {
            email: "",
            password: "",

            showPassword: false,

            formValidationMsgs: {
                failLogin: "",
                email: "",
                password: ""
            },

            loginPartialUrl: "api/user/login"
        }
    },

    methods: {
        resetValidation() {
            this.formValidationMsgs.failLogin = "";
            this.formValidationMsgs.email = "";
            this.formValidationMsgs.password = "";
        },

        async onLoginButtonClick() {
            const payload = {
                Email: this.email,
                Password: this.password
            };

            const response = await postAnonymous(this.loginPartialUrl, payload);

            this.resetValidation();

            if (response.status == 400 && response.errors) {
                if ("Email" in response.errors) {
                    this.formValidationMsgs.email = response.errors.Email.join('<br/>');
                }

                if ("Password" in response.errors) {
                    this.formValidationMsgs.password = response.errors.Password.join('<br/>');
                }
            }

            if (response.status === false) {
                this.formValidationMsgs.failLogin = response.message;
            }

            if (response.status === true) {
                setToken(response.data);
                router.push({ name: 'HomePage' });
            }
        },

        onRegisterButtonClick() {
            router.push({ name: 'RegisterPage' });
        }
    }
};
</script>
    
<style scoped>
.container-sm {
    max-width: 300px;
}

.form-check-input:focus,
.form-control:focus,
.form-select:focus {
    box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.15), 0 1px 1px rgba(0, 0, 0, 0.075);
    border-color: #6c757d;
}

.form-check-input:checked {
    background-color: #212529;
    border-color: #6c757d;
}

.form-text {
    margin-top: 0rem;
    margin-bottom: 1rem;
}
</style>  