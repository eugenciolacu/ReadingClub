<template>
    <div class="modal fade" :id="identifier" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" :id="identifier + '-title'">{{ title }}</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" :id="identifier + '-btn-close'"></button>
                </div>
                <div class="modal-body">
                    <span v-html="message"></span>
                </div>
                <div class="modal-footer">
                    <button @click.prevent="onClickConfirmButtonHandler" type="button" class="btn btn-dark" data-bs-dismiss="modal">Confirm</button>
                    <button type="button" class="btn btn-dark" data-bs-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: {
        identifier: {
            type: String,
            required: true
        },

        title: {
            type: String,
            required: true
        },

        message: {
            type: String,
            required: true
        }
    },

    emits: ['confirm-event'],

    setup(props, { emit }) {
        const onClickConfirmButtonHandler = () => {
            emit('confirm-event');
        }

        return {
            onClickConfirmButtonHandler
        }
    }
}
</script>

<style scoped>
.btn-close:focus,
.btn-dark:focus,
.form-select:focus {
    box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.15), 0 1px 1px rgba(0, 0, 0, 0.075);
    border-color: #6c757d;
}
</style>