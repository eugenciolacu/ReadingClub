<template>
    <div>
        <div class="container-sm">
            <div class="caption fw-bold">Statistics</div>

            <div class="row">
                <card title="Number of total uploaded books:" :amount="totalBooks"></card>
                <card title="Number of books uploaded by you:" :amount="uploadedByUser"></card>
                <card title="Number of books in yout reading list:" :amount="inUserReadingList"></card>
                <card title="Number of your read book:" :amount="readByUser"></card>
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
import { defineComponent, ref, onMounted  } from 'vue';
import CardStatisticsComponent from '../components/CardStatisticsComponent.vue';
import { getLoggedUserEmail } from '../mixins/getLoggedUser.js';
import { postAuthorized } from '../services/CrudService.js';
import InfoModalComponent from '../components/InfoModalComponent.vue';

export default defineComponent({
    name: 'HomePage',

    components: {
        Card: CardStatisticsComponent,
        InfoModal: InfoModalComponent
    },

    setup() {
        const totalBooks = ref(0);
        const uploadedByUser = ref(0);
        const inUserReadingList = ref(0);
        const readByUser = ref(0);

        const infoModalMessage = ref('');

        const getStatisticsUrl = 'api/book/getStatistics';


        onMounted(async () => {
            await fetchStatistics();
        });
        
        const fetchStatistics = async () => {
            let email = await getLoggedUserEmail();

            const payload = {
                userEmail: email
            }

            const response = await postAuthorized(getStatisticsUrl, payload);

            if (response.status === true) {
                totalBooks.value = response.data.totalBooks;
                uploadedByUser.value = response.data.uploadedByUser;
                inUserReadingList.value = response.data.inUserReadingList;
                readByUser.value = response.data.readByUser;
            } else {
                infoModalMessage.value = response.message;
                document.getElementById('hiddenLink').click();
            }
        }

        return {
            totalBooks, uploadedByUser, inUserReadingList, readByUser, infoModalMessage
        }
    }
});

</script>
    
<style scoped>
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