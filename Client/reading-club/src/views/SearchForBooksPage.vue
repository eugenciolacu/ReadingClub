<template>
    <div>
        <div class="container-sm">
            <div class="caption fw-bold">All posted books</div>

            <div class="row">
                <div class="col-sm-4">
                    <search identifier="titleFilter" filterPlaceholder="Search for Title" label="Title: "
                        :filterVal="fetchParams.titleFilter" @change-filter-value-event="changeFilterValueEventHandler">
                    </search>
                </div>
                <div class="col-sm-4">
                    <search identifier="authorsFilter" filterPlaceholder="Search for Authors" label="Authors: "
                        :filterVal="fetchParams.authorsFilter" @change-filter-value-event="changeFilterValueEventHandler">
                    </search>
                </div>
                <div class="col-sm-4">
                    <search identifier="isbnFilter" filterPlaceholder="Search for ISBN" label="ISBN: "
                        :filterVal="fetchParams.isbnFilter" @change-filter-value-event="changeFilterValueEventHandler">
                    </search>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-4">
                    <dropdown identifier="orderByFilter" :values="orderByFilterValues"
                        :defaultValue="fetchParams.orderByFilter" label="Order by: "
                        @change-filter-value-event="changeFilterValueEventHandler">
                    </dropdown>
                </div>
                <div class="col-sm-4">
                    <dropdown identifier="sortFilter" :values="sortFilterValues" :defaultValue="fetchParams.sortFilter"
                        label="Sort: " @change-filter-value-event="changeFilterValueEventHandler">
                    </dropdown>
                </div>
                <div class="col-sm-4">
                    <dropdown identifier="itemsPerPageFilter" :values="itemsPerPageFilterValues"
                        :defaultValue="fetchParams.itemsPerPageFilter" label="Items per page: "
                        @change-filter-value-event="changeFilterValueEventHandler">
                    </dropdown>
                </div>
            </div>

            <div style="margin: 2rem"></div>

            <div class="row">
                <book v-for="book in booksData" :key="book.id" :book="book"
                    @show-book-details-event="onShowBookDetailsEventHandler"
                    @hide-book-details-event="onHideBookDetailsEventHandler"
                    @add-book-to-reading-list-event="onAddBookToReadingListEventHandler"
                    @book-download-error-event="onBookDownloadErrorEventHandler">
                </book>

                <span v-if="booksData.length == 0" class="d-flex justify-content-center">No records to view</span>
            </div>

            <div v-if="booksData.length !== 0" class="d-flex justify-content-center">
                <pagination :total="totalBooks" :items-per-page="fetchParams.itemsPerPageFilter" :page="fetchParams.page"
                    @page-changed-event="onPageChangeEventHandler"></pagination>
            </div>
        </div>

        <!-- this hidden link is used to call info-modal  -->
        <a href="#" ref="hiddenLink" id="hiddenLink" data-bs-toggle="modal" data-bs-target="#info"></a>

        <Teleport to="body">
            <info-modal identifier="info" :message="infoModalMessage"></info-modal>
        </Teleport>
    </div>
</template>
    
<script>
import router from '../router';
import SearchComponent from '../components/SearchComponent.vue';
import DropdownComponent from '../components/DropdownComponent.vue';
import BookComponent from '../components/BookComponent.vue';
import { postAuthorized } from '../services/CrudService';
import getLoggedUser from '../mixins/getLoggedUser.js';
import PaginationComponent from '../components/PaginationComponent.vue';
import InfoModalComponent from '../components/InfoModalComponent.vue';

export default {
    name: 'SearchForBooksPage',

    components: {
        Search: SearchComponent,
        Dropdown: DropdownComponent,
        Book: BookComponent,
        Pagination: PaginationComponent,
        InfoModal: InfoModalComponent
    },

    data() {
        return {
            booksData: [],
            totalBooks: 0,

            fetchParams: {
                titleFilter: '',
                authorsFilter: '',
                isbnFilter: '',
                orderByFilter: 'Title',
                sortFilter: 'Ascending',
                itemsPerPageFilter: '5',
                page: 1
            },

            orderByFilterValues: ['Title', 'Authors', 'ISBN'],
            sortFilterValues: ['Ascending', 'Descending'],
            itemsPerPageFilterValues: ['5', '10', '20', '50'],

            infoModalMessage: '',

            urls: {
                getPagedSortedAndFilteredBooksUrl: 'api/book/getPagedSearchPage',
                addBookToReadingListUrl: 'api/book/addToReadingList'
            },
        }
    },

    mixins: [getLoggedUser],

    created() {
        this.fetchParams.page = parseInt(this.$route.params.page);

        let queryParams = this.$route.query;
        let defaultQuery = this.$route.meta.defaultQuery;

        this.fetchParams.titleFilter = queryParams.title === undefined ? defaultQuery.title : queryParams.title;
        this.fetchParams.authorsFilter = queryParams.authors === undefined ? defaultQuery.authors : queryParams.authors;
        this.fetchParams.isbnFilter = queryParams.isbn === undefined ? defaultQuery.isbn : queryParams.isbn;
        this.fetchParams.orderByFilter = queryParams.orderBy === undefined ? defaultQuery.orderBy : queryParams.orderBy;
        this.fetchParams.sortFilter = queryParams.sort === undefined ? defaultQuery.sort : queryParams.sort;
        this.fetchParams.itemsPerPageFilter = queryParams.items === undefined ? defaultQuery.items : queryParams.items;
        // fix dropdowns if inexistent option is added from request param
        if (this.orderByFilterValues.includes(this.fetchParams.orderByFilter) === false) {
            this.fetchParams.orderByFilter = defaultQuery.orderBy;
        }
        if (this.sortFilterValues.includes(this.fetchParams.sortFilter) === false) {
            this.fetchParams.sortFilter = defaultQuery.sort;
        }
        if (this.itemsPerPageFilterValues.includes(this.fetchParams.itemsPerPageFilter) === false) {
            this.fetchParams.itemsPerPageFilter = defaultQuery.items;
        }
    },

    mounted() {
        this.fetchBooks();
    },

    methods: {
        onPageChangeEventHandler(page) {
            this.fetchParams.page = page;
            this.$route.params.page = page;

            this.pushRoute();
        },

        changeFilterValueEventHandler(identifier, filterValue) {
            let queryParams = this.$route.query;

            switch (identifier) {
                case 'titleFilter':
                    this.fetchParams.titleFilter = filterValue;
                    queryParams.title = filterValue;
                    this.updateRoute('title');
                    break;
                case 'authorsFilter':
                    this.fetchParams.authorsFilter = filterValue;
                    queryParams.authors = filterValue;
                    this.updateRoute('authors');
                    break;
                case 'isbnFilter':
                    this.fetchParams.isbnFilter = filterValue;
                    queryParams.isbn = filterValue;
                    this.updateRoute('isbn');
                    break;
                case 'orderByFilter':
                    this.fetchParams.orderByFilter = filterValue;
                    queryParams.orderBy = filterValue;
                    this.updateRoute('orderBy');
                    break;
                case 'sortFilter':
                    this.fetchParams.sortFilter = filterValue;
                    queryParams.sort = filterValue;
                    this.updateRoute('sort');
                    break;
                case 'itemsPerPageFilter':
                    this.fetchParams.itemsPerPageFilter = filterValue;
                    queryParams.items = filterValue;
                    this.updateRoute('items');
                    break;
            }

            this.fetchParams.page = 1;
            this.$route.params.page = this.fetchParams.page;

            this.pushRoute();
        },

        updateRoute(fieldName) {
            let queryParams = this.$route.query;
            let defaultQuery = this.$route.meta.defaultQuery;

            if (queryParams[fieldName] === '' || queryParams[fieldName] === defaultQuery[fieldName]) {
                delete queryParams[fieldName]
            }
        },

        async pushRoute() {
            router.push({ name: this.$route.name, params: this.$route.params, query: this.$route.query });

            await this.fetchBooks();
        },

        async fetchBooks() {
            let email = await this.getLoggedUserEmail();

            const payload = {
                pageSize: parseInt(this.fetchParams.itemsPerPageFilter),
                page: this.fetchParams.page,
                orderBy: this.fetchParams.orderByFilter,
                orderDirection: this.fetchParams.sortFilter,
                filters: {
                    title: this.fetchParams.titleFilter,
                    authors: this.fetchParams.authorsFilter,
                    isbn: this.fetchParams.isbnFilter
                },
                userEmail: email
            };

            const response = await postAuthorized(this.urls.getPagedSortedAndFilteredBooksUrl, payload);

            if (response.status === true) {
                this.booksData = response.data.items;
                this.totalBooks = response.data.totalItems;
            } else {
                this.infoModalMessage = response.message;
                this.$refs.hiddenLink.click();
            }

            window.scroll(0, 0);
        },

        onShowBookDetailsEventHandler(bookId) {
            router.push({ name: 'BookDetailsForSearchPage', params: { id: bookId }, query: this.$route.query });
        },

        onHideBookDetailsEventHandler() {
            router.push({ name: 'SearchForBooksPage', params: { page: this.fetchParams.page }, query: this.$route.query });
        },

        async onAddBookToReadingListEventHandler(bookId) {
            let email = await this.getLoggedUserEmail();

            const payload = {
                userEmail: email,
                bookId: bookId
            };

            const response = await postAuthorized(this.urls.addBookToReadingListUrl, payload);

            let addedBook = this.booksData.find(obj => obj.id === bookId);

            if (response.status === true) {
                addedBook.isInReadingList = true;
            } else {
                this.infoModalMessage = response.message;
                this.$refs.hiddenLink.click();
            }
        },

        onBookDownloadErrorEventHandler(message) {
            this.infoModalMessage = message;
            this.$refs.hiddenLink.click();
        }
    }
};
</script>
  
<style scoped>
.list-group {
    --bs-list-group-border-width: 0px;
}

.list-group-item {
    margin-bottom: 20px;
}

.caption {
    padding-top: 0.5rem;
    padding-bottom: 0.5rem;
    color: #6c757d;
    text-align: left;
}

#hiddenLink {
    display: none;
}
</style>
    