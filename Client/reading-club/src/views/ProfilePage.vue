<template>
    <div class="container-sm">
        <div class="caption fw-bold">User profile</div>

        <div class="row">
            <div class="block">
                <p class="mb-1 fw-bold">User name:</p>
                <p class="mb-1">{{ userName }}</p>
            </div>

            <div class="block">
                <p class="mb-1 fw-bold">User email:</p>
                <p class="mb-1">{{ userEmail }}</p>
            </div>

            <div class="block">
                <button type="button" class="btn btn-dark" data-bs-toggle="modal" data-bs-target="#confirm">Delete
                    user</button>
                <button @click="completeInputsOnEditModal" type="button" class="btn btn-dark" data-bs-toggle="modal"
                    data-bs-target="#editResource">Update user</button>
            </div>
        </div>

        <Teleport to="body">
            <confirmation-modal identifier="confirm" title="Delete user" :message="modalConfirmMsg"
                @confirm-event="onConfirmDeleteEventHandler"></confirmation-modal>
        </Teleport>

        <!-- Modal edit -->
        <div class="modal fade" id="editResource" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1"
            aria-labelledby="editResourceLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h1 class="modal-title fs-5" id="editResourceLabel">Edit profile</h1>
                        <button id="editResourceCloseBtn" type="button" class="btn-close" data-bs-dismiss="modal"
                            aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label for="editUserName" class="form-label">User name</label>
                            <input v-model="editUserName" type="text" class="form-control" id="editUserName"
                                placeholder="User name">
                            <div v-if="editUserNameValidationMsg != ''" class="form-text">
                                <span v-html="editUserNameValidationMsg"></span>
                            </div>
                        </div>
                        <div class="mb-3">
                            <label for="editEmail" class="form-label">Email</label>
                            <input v-model="editUserEmail" type="text" class="form-control" id="editEmail"
                                placeholder="Email">
                            <div v-if="editUserEmailValidationMsg" class="form-text">
                                <span v-html="editUserEmailValidationMsg"></span>
                            </div>
                        </div>
                        <div class="mb-3 form-check">
                            <input v-model="isEditPassword" type="checkbox" class="form-check-input" id="editPasswordInput">
                            <label class="form-check-label" for="editPasswordInput"> Edit password </label>
                        </div>
                        <div v-if="isEditPassword" class="mb-3">
                            <label for="passwordInput" class="form-label">New password</label>
                            <input v-model="editPassword" :type="showPassword ? 'text' : 'password'" class="form-control"
                                id="passwordInput" placeholder="Password">
                            <div v-if="editPasswordValidationMsg != ''" class="form-text">
                                <span v-html="editPasswordValidationMsg"></span>
                            </div>
                        </div>
                        <div v-if="isEditPassword" class="mb-3">
                            <label for="confirmPasswordInput" class="form-label">Confirm password</label>
                            <input v-model="editConfirmPassword" :type="showPassword ? 'text' : 'password'"
                                class="form-control" id="confirmPasswordInput" placeholder="Confirm password">
                            <div v-if="editConfirmPasswordValidationMsg != ''" class="form-text">
                                <span v-html="editConfirmPasswordValidationMsg"></span>
                            </div>
                        </div>

                        <div v-if="failUpdateMsg != ''" class="form-text">
                            {{ failUpdateMsg }}
                        </div>

                        <div v-if="isEditPassword" class="mb-3 form-check">
                            <input v-model="showPassword" type="checkbox" class="form-check-input" id="showPasswordInput">
                            <label class="form-check-label" for="showPasswordInput"> Show password </label>
                        </div>

                    </div>
                    <div class="modal-footer">
                        <button @click.prevent="onClickEditButtonHandler" type="button" class="btn btn-dark">Edit</button>
                        <button type="button" class="btn btn-dark" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- this hidden link is used to call info-modal  -->
        <a href="#" id="hiddenLinkInfo" class="hiddenLink" data-bs-toggle="modal" data-bs-target="#info"></a>

        <Teleport to="body">
            <info-modal identifier="info" :message="modalInfoMsg"></info-modal>
        </Teleport>
    </div>
</template>
    
<script>
import { ref, onMounted, watch } from 'vue';
import { postAuthorized, deleteAuthorized, putAuthorized } from '../services/CrudService.js';
import router from '@/router/index.js';
import { removeToken } from '@/services/AuthenticationService';
import InfoModalComponent from '../components/InfoModalComponent.vue';
import ConfirmationModalComponent from '../components/ConfirmationModalComponent.vue';
import { checkIfUserIsAuthenticated } from '@/services/AuthenticationService';

export default {

    components: {
        InfoModal: InfoModalComponent,
        ConfirmationModal: ConfirmationModalComponent
    },

    setup() {
        const userName = ref('');
        const userEmail = ref('');

        const editUserName = ref('');
        const editUserEmail = ref('');
        const isEditPassword = ref(false);
        const editPassword = ref('');
        const editConfirmPassword = ref('');

        const editUserNameValidationMsg = ref('');
        const editUserEmailValidationMsg = ref('');
        const editPasswordValidationMsg = ref('');
        const editConfirmPasswordValidationMsg = ref('');

        const showPassword = ref(false);

        const failUpdateMsg = ref('');

        const modalInfoMsg = ref('');
        const modalConfirmMsg = ref(`
            <p>
                Your account will be deleted from the database. Books uploaded by you will not be deleted from
                the database, but won't be associated with you. The books will be associated with the <b>"anonymous"</b> user.
                <br />
                Do you confirm?
            </p>
        `);

        const getUserDetailsUrl = 'api/user/getFullDetailsOfLoggedUser';
        const deleteUserUrl = 'api/user/deleteLoggedUser';
        const updateUserUrl = 'api/user/update';

        watch(isEditPassword, (newIsEditPassword, oldIsEditPassword) => {
            if(newIsEditPassword === false && oldIsEditPassword === true){
                editPassword.value = '';
                editConfirmPassword.value = '';
            }
        });

        onMounted(async () => {
            fetchUser();
        });

        const onConfirmDeleteEventHandler = async () => {
            let isAuthenticated = await checkIfUserIsAuthenticated();

            if (isAuthenticated === false){
                router.push({ name: 'LoginPage' });
                return;
            }

            const deleteResponse = await deleteAuthorized(deleteUserUrl);

            if (deleteResponse.status === true) {
                removeToken();

                let confirmationCloseBtn = document.getElementById('confirm-btn-close');
                confirmationCloseBtn.click();

                router.push({ name: 'LoginPage' });
            } else {
                modalInfoMsg.value = deleteResponse.message;
                let hiddenLink = document.getElementById('hiddenLinkInfo');
                hiddenLink.click();
            }
        }

        const completeInputsOnEditModal = async () => {
            editUserName.value = userName.value;
            editUserEmail.value = userEmail.value;
        }

        const resetEditValidationMessages = () => {
            editUserNameValidationMsg.value = '';
            editUserEmailValidationMsg.value = '';
            editPasswordValidationMsg.value = '';
            editConfirmPasswordValidationMsg.value = '';
            failUpdateMsg.value = '';
        }

        const onClickEditButtonHandler = async () => {
            let isAuthenticated = await checkIfUserIsAuthenticated();

            if (isAuthenticated === false){
                let editResourceCloseBtn = document.getElementById('editResourceCloseBtn');
                editResourceCloseBtn.click();
                router.push({ name: 'LoginPage' });
                return;
            }

            const payload = {
                UserName: editUserName.value,
                Email: editUserEmail.value,
                Password: isEditPassword.value ? editPassword.value : 'dummy string',
                ConfirmPassword: isEditPassword.value ? editConfirmPassword.value : 'dummy string',
                OldEmail: userEmail.value,
                IsEditPassword: isEditPassword.value 
            }

            let response = await putAuthorized(updateUserUrl, payload);

            resetEditValidationMessages();

            if (response.status === 400 && response.errors) {
                if ("UserName" in response.errors) {
                    editUserNameValidationMsg.value = response.errors.UserName.join('<br/>');
                }

                if ("Email" in response.errors) {
                    editUserEmailValidationMsg.value = response.errors.Email.join('<br/>');
                }

                if ("Password" in response.errors) {
                    editPasswordValidationMsg.value = response.errors.Password.join('<br/>');
                }
                
                if ("ConfirmPassword" in response.errors) {
                    editConfirmPasswordValidationMsg.value = response.errors.ConfirmPassword.join('<br/>');
                }   
            }

            if (response.newStatus === true) {
                let editResourceCloseBtn = document.getElementById('editResourceCloseBtn');
                editResourceCloseBtn.click();

                modalInfoMsg.value = "<div>Your account has been successfully updated.</div>";
                let hiddenLink = document.getElementById('hiddenLinkInfo');
                hiddenLink.click();

                fetchUser();
            }
        }

        const fetchUser = async () => {
            const userDetailsResponse = await postAuthorized(getUserDetailsUrl, {});

            if (userDetailsResponse.status === true) {
                userName.value = userDetailsResponse.data.userName;
                userEmail.value = userDetailsResponse.data.email;
            }
        }

        return {
            userName, userEmail, onConfirmDeleteEventHandler,
            editUserName, editUserEmail, editUserNameValidationMsg,
            editUserEmailValidationMsg, completeInputsOnEditModal, onClickEditButtonHandler,
            isEditPassword, editPassword, editConfirmPassword, editPasswordValidationMsg,
            editConfirmPasswordValidationMsg, showPassword, failUpdateMsg, modalInfoMsg, modalConfirmMsg
        }
    }
};
</script>
    
<style scoped>
.caption {
    padding-top: 0.5rem;
    padding-bottom: 0.5rem;
    color: #6c757d;
    text-align: left;
}

.block {
    margin-bottom: 1rem;
}

.block button {
    margin-right: 20px;
    margin-bottom: 10px;
}

.form-check-input:checked {
    background-color: #212529;
    border-color: #6c757d;
}

.form-check-input:focus,
.btn-close:focus,
.btn-dark:focus,
.form-control:focus,
.form-select:focus {
    box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.15), 0 1px 1px rgba(0, 0, 0, 0.075);
    border-color: #6c757d;
}

.hiddenLink {
    display: none;
}
</style>