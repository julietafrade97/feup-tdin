<template>
    <div>
        <div class="title-wrapper">
            <hr>
            <h1>Books</h1>
            <hr>
        </div>
        <div class="table-wrapper">
            <v-text-field
            class="search"
            v-model="search"
            append-icon="search"
            label="Search"
            single-line
            hide-details
            ></v-text-field>
            <v-data-table
                v-model="selected"
                :headers="headers"
                :items="books"
                :search="search"
                item-key="id"
                select-all
                hide-actions
                class="elevation-1">
                <template v-slot:items="props">
                    <td>
                        <v-checkbox
                        v-model="props.selected"
                        primary
                        hide-details
                        ></v-checkbox>
                    </td>
                    <td>{{ props.item.id }}</td>
                    <td class="text-xs-right">{{ props.item.title }}</td>
                    <td class="text-xs-right">{{ props.item.unitprice }} €</td>
                    <td class="text-xs-right">{{ props.item.stock }}</td>
                    <td class="text-xs-right">
                        <v-text-field
                            v-model="props.item.qnt"
                            class="desired-input"
                            :rules="[integerRule]"
                            single-line
                        ></v-text-field>
                    </td>
                    <td class="text-xs-right">{{ props.item.totalprice }} €</td>
                </template>
            </v-data-table>
        </div>
        <div class="total-price"><p>{{totalPrice}} €</p></div>
        <div class="text-xs-right button-wrapper">
            <v-btn round color="accent" v-on:click.native="buy">Buy</v-btn>
        </div>
        <v-snackbar
            v-model="snackbar"
            :color="snackcolor"
            :timeout=5000
        >
            {{ snacktext }}
            <v-btn dark flat @click="snackbar = false">Close </v-btn>
        </v-snackbar>
    </div>
 </template>
  
<script>
    import axios from 'axios'

    export default {
        name: 'books',
        data () {
        return {
            snackbar: false,
            snackcolor: '',
            snacktext: '',

            integerRule: v=> /^[0-9]*$/.test(v) || 'Input must be a integer',
            search: '',
            selected: [],
            headers: [
            { text: 'ID', align: 'left', value:'id'},
            { text: 'Title', align: 'right', value: 'title' },
            { text: 'Unit Price', align: 'right', value: 'unitprice' },
            { text: 'Stock', align: 'right', value: 'stock' },
            { text: 'Desired Quantity', align: 'right', value: 'qnt' },
            { text: 'Total Price', align: 'right', value:'totalprice'  },
            ],
            books : [],
            oldbooks:[],
            totalPrice: 0,
        }
    },
    created () {
      if (!this.$session.exists()) {
        this.$router.push('/')
      }
    },
    watch: {
        books: {
            handler: function (after, before) {
                var vm = this;
               
                    var changed = after.filter( function( p, idx ) {
                        return Object.keys(p).some( function( prop ) {
                            return p[prop] !== vm.$data.oldbooks[idx][prop];
                        })
                    })

                    for(let i = 0; i < changed.length; i++){
                        
                        var difference = changed[0].totalprice - this.oldbooks.find(x => x.id ===  changed[0].id).totalprice;
                        
                        vm.setValue();
                        var obj = changed[0];
                        
                        if(!isNaN(obj.qnt) && Number.isInteger(parseFloat(obj.qnt,10))){
                            obj.totalprice=obj.qnt*obj.unitprice;
                        }
                        else{
                            obj.totalprice=0;
                        }
                        let tp_temp = this.totalPrice + difference;

                        this.totalPrice = parseFloat(tp_temp).toFixed( 2 ) * 1;                    
                    }            
            },
            deep: true
        }
    },
    mounted(){
        this.getAllBooks(this);   
        this.getAllBooksInterval();
        this.setValue();
    },
    methods: {
        getAllBooks(self){
            console.log("Get All Books called");
            let vm=self;

            axios.get('http://localhost:5000/api/store/getAllBooks')
            .then(function (response) {
                // handle success
                vm.books=[];
                let allBooks = response.data;
                let tempBooks =[];
                for(let i=0; i< allBooks.length; i++){
                    let book = {
                        id: allBooks[i].id,
                        title: allBooks[i].title,
                        unitprice: allBooks[i].unitprice,
                        stock: allBooks[i].stock,
                        qnt: '',
                        totalprice: 0,
                    }

                    tempBooks.push(book);
                }
                
                vm.setValueWithArray(tempBooks);
                vm.books=tempBooks;
                //vm.setValueWithArray(tempBooks)

                vm.selected=[];
                vm.totalPrice = 0;

            })
            .catch(function (error) {
                // handle error
                if(!error.response.status==404){
                    console.log(error);
                    vm.snacktext='Something went wrong.';
                    vm.snackcolor='error';
                    vm.snackbar=true;
                }
            })
        },
        getAllBooksInterval(){
            var self= this;
            setInterval(function () {
                self.getAllBooks(self);
            },20000);
        },
       setValue: function() {
            var _ = require('lodash');
            this.$data.oldbooks = _.cloneDeep(this.$data.books);
        },
        setValueWithArray: function(books) {
            var _ = require('lodash');
            this.$data.oldbooks = _.cloneDeep(books);
        },
        buy(event) {
            //TODO: get client id
            console.log(this.selected);
           
            let vm=this;
            for(let i = 0; i < this.selected.length; i++){
                if(!isNaN(this.selected[i].qnt) && Number.isInteger(parseFloat(this.selected[i].qnt, 10))){
                    let floatQnt = parseFloat(this.selected[i].qnt, 10);
            
                    axios.post('http://localhost:5000/api/store/createOrder', 
                    {
                        clientEmail: this.$session.get('email'),
                        bookId: this.selected[i].id,
                        quantity: floatQnt,
                        local: 'webapp'
                    })
                    .then(function (response) {
                        // handle success
                        vm.snacktext='Books successfully ordered.';
                        vm.snackcolor='success';
                        vm.snackbar=true;
    
                    }).catch(function (error) {
                        // handle error
                        console.log(error);
                        vm.snacktext='Something went wrong.';
                        vm.snackcolor='error';
                        vm.snackbar=true;

                    })
                }       
            }  
        }
    },
    
  }
</script>
  
<style>
    div.title-wrapper{
        display:flex;
        align-items: center;
        margin-top:1em;
    }
    div.title-wrapper hr{
        width: 100%;
        border-top: .2em solid #76a5af;
    }
    div.title-wrapper hr:first-child{
        width: 17.6%;
        height:1px;
    }
    div.title-wrapper h1{
        color: #76a5af;
        font-size: 2em;
        padding-right: .6em;
        padding-left: .6em;
    }
    div.table-wrapper{
        display: flex;
        flex-direction: column;
        margin-left: 15%;
        margin-right: 15%;
        margin-top:1.5em;
    }
    div.total-price{
        margin-left: 15%;
        margin-right: 15%;
        margin-top:1em;
    }

    div.total-price p{
        width: 100%;
        text-align: right;
        font-weight: bold;
        font-size: 1.7em;
        padding-right: 1em;
    }

    div.button-wrapper{
        margin-left: 15%;
        margin-right: 15%;
        margin-top:1em;  
    }

    div.button-wrapper button{
        font-weight: 600;
        height: 2.6em;
        width: 5.3em;
        font-size: 1.3em;
        margin-bottom:3em;
    }

    div.table-wrapper .v-text-field.search{
        margin-bottom:1.5em;
        width: 50%;
        align-self: center;
    }

     div.table-wrapper .v-text-field.search label{
        font-size: 1.2em;
    }


    div.table-wrapper .v-text-field.search input{
        font-size: 1.2em;
    }

    th.column{
        font-weight: bold !important;
        font-size: 1.2em !important;
    }
    th.column i{
        font-size: 1.2em !important;
    }

    td{
        font-size: 1.2em !important;
        padding: 1em;
    }

    .desired-input{
        width: 60%;
        margin-right: 0;
        margin-left: auto;
        text-align: right;
        padding:0;
        padding-bottom: .5em
    }

    .desired-input input{
        text-align: right;
        font-size:1.1em;
    }

</style>