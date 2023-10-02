<template>
    <div class="col-md-3">
        <div class="card">
            <div class="card-body">
                <p class="card-title">{{ title }}</p>
                <h3 class="card-text"> {{ amount }}</h3>
            </div>
        </div>
    </div>
</template>

<script>

export default {
    props: {
        title: {
            type: String,
            required: true
        },

        amount: {
            type: Number,
            required: true
        }
    },

    setup() {

    }
}

</script>

<style scoped>
.card {
    display: flex;
    flex-direction: column;
    justify-content: center;
    margin-bottom: 20px;
    min-height: 300px;
    background-color: #dee2e6;
}

.card-body {
    flex: 1;
    display: flex;
    flex-direction: column;
    justify-content: center;
}

@media (max-width: 767px) {
    .card {
        min-height: 0;
    }
}
</style>