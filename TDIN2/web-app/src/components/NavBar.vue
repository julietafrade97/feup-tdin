<template>
    <nav>
        <h1 class="logo"><span>Book</span>Shop</h1>
        <div>
            <router-link class="nav-link" to="/books">Books</router-link>
            <router-link  class="nav-link" to="/orders">My Orders</router-link>
            <router-link  class="nav-link" to="" v-on:click.native="logout">Logout</router-link>
        </div>
    </nav>
</template>
    
<script>
    export default {
        name: 'navbar',
        methods: {
            logout: function () {
                this.$session.destroy()
                this.$router.push('/')
            }
        }
    }
</script>
    
<style>
    nav{
        display: flex;
        justify-content: space-between;
        margin-right: 15%;
        margin-left: 15%;
        margin-top: 1em;
        align-items: flex-end;
        
    }
    h1.logo{
        font-weight: lighter;
        font-size: 3em;
        color: #45818e;
        user-select: none;
        font-style: underline;
    }
    h1.logo span{
        font-weight: 800;
    }
    a.nav-link{
        font-size: 1.5em;
        color: #45818e;
        margin-left:1em;
        text-decoration: none;
        font-weight: lighter;
    }
    a.router-link-active{
        color: #45818e;;
        font-weight: bold;
    }
    a.nav-link:last-child{
        font-style:italic;
    }



</style>