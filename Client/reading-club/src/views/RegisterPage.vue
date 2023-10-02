<template>
    <div class="container-sm">
        <form>
            <div class="mb-3">
                <label for="userNameInput" class="form-label">User name</label>
                <input v-model="username" type="text" class="form-control" id="userNameInput" placeholder="User name">
                <div v-if="formValidationMsgs.username != ''" class="form-text">
                    <span v-html="formValidationMsgs.username"></span>
                </div>
            </div>

            <div class="mb-3">
                <label for="emailInput" class="form-label">Email</label>
                <input v-model="email" type="email" class="form-control" id="emailInput" placeholder="Email">
                <div v-if="formValidationMsgs.email != ''" class="form-text">
                    <span v-html="formValidationMsgs.email"></span>
                </div>
            </div>

            <div class="mb-3">
                <label for="passwordInput" class="form-label">Password</label>
                <input v-model="password" :type="showPassword ? 'text' : 'password'" class="form-control" id="passwordInput"
                    placeholder="Password">
                <div v-if="formValidationMsgs.password != ''" class="form-text">
                    <span v-html="formValidationMsgs.password"></span>
                </div>
            </div>

            <div class="mb-3">
                <label for="confirmPasswordInput" class="form-label">Confirm password</label>
                <input v-model="confirmPassword" :type="showPassword ? 'text' : 'password'" class="form-control"
                    id="confirmPasswordInput" placeholder="Confirm password">
                <div v-if="formValidationMsgs.confirmPassword != ''" class="form-text">
                    <span v-html="formValidationMsgs.confirmPassword"></span>
                </div>
            </div>

            <div v-if="formValidationMsgs.failRegister != ''" class="form-text">
                {{ formValidationMsgs.failRegister }}
            </div>

            <div class="mb-3 form-check">
                <input v-model="showPassword" type="checkbox" class="form-check-input" id="showPasswordInput">
                <label class="form-check-label" for="showPasswordInput"> Show password </label>
            </div>

            <div>
                <button @click.prevent="onRegisterButtonClick" type="submit"
                    class="btn btn-dark mb-3 container">Register</button>
            </div>
        </form>
    </div>
</template>
    
<script>
import router from '@/router/index.js';
import { postAnonymous } from '@/services/CrudService';

export default {
    name: 'RegisterPage',

    data() {
        return {
            username: "",
            email: "",
            password: "",
            confirmPassword: "",

            showPassword: false,

            formValidationMsgs: {
                failRegister: "",
                username: "",
                email: "",
                password: "",
                confirmPassword: ""
            },

            registerPartialUrl: "api/user/create"
        }
    },

    methods: {
        resetValidation() {
            this.formValidationMsgs.failRegister = "";
            this.formValidationMsgs.username = "";
            this.formValidationMsgs.email = "";
            this.formValidationMsgs.password = "";
            this.formValidationMsgs.confirmPassword = "";
        },

        async onRegisterButtonClick() {
            const payload = {
                UserName: this.username,
                Email: this.email,
                Password: this.password,
                ConfirmPassword: this.confirmPassword
            };

            const response = await postAnonymous(this.registerPartialUrl, payload);

            this.resetValidation();

            if (response.status == 400 && response.errors) {
                if ("UserName" in response.errors) {
                    this.formValidationMsgs.username = response.errors.UserName.join('<br/>');
                }

                if ("Email" in response.errors) {
                    this.formValidationMsgs.email = response.errors.Email.join('<br/>');
                }

                if ("Password" in response.errors) {
                    this.formValidationMsgs.password = response.errors.Password.join('<br/>');
                }

                if ("ConfirmPassword" in response.errors) {
                    this.formValidationMsgs.confirmPassword = response.errors.ConfirmPassword.join('<br/>');
                }
            }

            if (response.status === false) {
                this.formValidationMsgs.failRegister = response.message;
            }

            if (response.status === true) {
                router.push({ name: 'LoginPage' });
            }
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
    