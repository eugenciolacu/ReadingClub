<template>
    <ul class="pagination">
        <li class="page-item" :class="{ disabled: page == 1 }">
            <a @click="onClickFirstPage" class="page-link" :disabled="1===1">
                <i class="bi bi-chevron-double-left"></i>
            </a>
        </li>
        <li class="page-item" :class="{ disabled: page <= 5 }">
            <a @click="onClickPreviousPage" class="page-link">
                -5
            </a>
        </li> 
        <span>&nbsp;&nbsp;&nbsp;</span>
        <li v-for="page in middlePagesRange" :key="page" class="page-item">
            <a @click="onClickPage(page)" class="page-link" 
                :class="{ active: page == this.page }">{{ page }}</a>
        </li>
        <span>&nbsp;&nbsp;&nbsp;</span>
        <li class="page-item" :class="{ disabled: maxPage - 4 <= page }">
            <a @click="onClickNextPage" class="page-link">
                +5
            </a>
        </li>
        <li class="page-item" :class="{ disabled: page == maxPage }">
            <a @click="onClickLastPage" class="page-link">
                <i class="bi bi-chevron-double-right"></i>
            </a>
        </li>
    </ul>
</template>
    
<script>

export default {
    name: 'PaginationComponent',

    props: {
        total: {
            type: Number,
            required: true
        },
        itemsPerPage: {
            type: String,
            required: true
        },
        page: {
            type: Number,
            required: true
        }
    },

    emits: ['page-changed-event'],

    computed: {
        maxPage() {
            let quotient = Math.floor(this.total / this.itemsPerPage);
            let reminder = this.total % this.itemsPerPage;

            return reminder === 0 ? quotient : quotient + 1;
        },

        middlePagesRange() {
            let arr = [];

            if (this.page <= 3){
                for (let i = 1; i <= this.maxPage && i <= 5; i++){
                    arr.push(i);
                }
            } 
            else if (this.maxPage - this.page <= 2){
                for (let i = this.maxPage - 4; i <= this.maxPage; i++){
                    if (i >= 1 && i <= this.maxPage){
                        arr.push(i);
                    }
                }
            } else {
                for (let i = this.page - 2; i <= this.page + 2; i++){
                    if (i >= 1 && i <= this.maxPage){
                        arr.push(i);
                    }
                }
            }

            return arr;
        }
    },

    methods: {
        onClickFirstPage() {
            this.$emit('page-changed-event', 1);
        },
        onClickPreviousPage() {
            this.$emit('page-changed-event', this.page - 5 <= 1 ? 1 : this.page - 5);
        },
        onClickPage(page) {
            this.$emit('page-changed-event', page);
        },
        onClickNextPage() {
            this.$emit('page-changed-event', this.page + 5 >= this.maxPage ? this.maxPage : this.page + 5);
        },
        onClickLastPage() {
            this.$emit('page-changed-event', this.maxPage);
        },
        isPageActive(page) {
            return this.page === page;
        }
    }
};
</script>
  
<style scoped>
ul {
    margin: 0;
    margin-bottom: 2rem;
}

.page-link {
    margin: 10px;
    border-radius: 0.375rem !important;
    --bs-pagination-border-width: 1px;
    --bs-pagination-color: #212529;
    --bs-pagination-active-bg: #6c757d;
    --bs-pagination-active-border-color: #dee2e6;
    --bs-pagination-hover-color: #212529;
}

.page-link:hover {
    color: #212529;
}
</style>
    




