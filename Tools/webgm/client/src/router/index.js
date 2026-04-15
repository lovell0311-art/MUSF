import { createRouter, createWebHistory } from 'vue-router'
import IndexView from "../views/index.vue"
import Register from "../views/Register.vue"
import Login from "../views/login.vue"
import NotFound from "../views/404.vue"
import Home from "../views/home.vue"
import Test from "../views/test.vue"
import jwt_decode from "jwt-decode"
import {usePermissStore} from '@/store/permiss'
import { EPermiss } from '../../../common/permiss';


const LoginPath = "login";



const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: `/${LoginPath}`,
    },
    {
      path: '/register',
      name:'register',
      component: Register
    },
    {
      path: '/' + LoginPath,
      name:'login',
      component: Login
    },
    { path:'/404', name:'404',permiss:"user",component: NotFound },
    { path:'/403', name:'403',permiss:"user",component: ()=> import("../views/403.vue") },
    {
      path: '/test/:id',
      name:'test',
      component: Test
    },
    {
      path: '/index',
      name:'index',
      component: IndexView,
      children:[
        {path:'',name:'home',component:Home},
        {
          path:'/4041',
          name:'4041',
          component:NotFound},
          {
            path:'/dashboard_admin',
            name:'dashboard_admin',
            meta:{
              permiss:EPermiss.dashboard_admin,
            },
            component: ()=> import("../views/index/dashboard_admin.vue")
          },
          {
            path:'/dashboard_proxy',
            name:'dashboard_proxy',
            meta:{
              permiss:EPermiss.dashboard_proxy,
            },
            component: ()=> import("../views/index/dashboard_proxy.vue")
          },
          {
            path:'/search_account',
            name:'search_account',
            meta:{
              permiss:EPermiss.search_account,
            },
            component: ()=> import("../views/index/search_account.vue")
          },
          {
            path:'/search_role',
            name:'search_role',
            meta:{
              permiss:EPermiss.search_role,
            },
            component: ()=> import("../views/index/search_role.vue")
          },
          {
            path:'/server_manager',
            name:'server_manager',
            meta:{
              permiss:EPermiss.server_manager,
            },
            component: ()=> import("../views/index/server_manager.vue")
          },
          {
            path:'/cdkey_type',
            name:'cdkey_type',
            meta:{
              permiss:EPermiss.cdkey,
            },
            component: ()=> import("../views/index/cdkey_type.vue"),
          },
          
          {
            path:'/cdkey_type/z:zone_id/:reward_type',
            name:'cdkey',
            meta:{
              permiss:EPermiss.cdkey,
            },
            component: ()=> import("../views/index/cdkey_type/cdkey.vue"),
          },
          {
            path:'/user_manager',
            name:'user_manager',
            meta:{
              permiss:EPermiss.user_manager,
            },
            component: ()=> import("../views/index/user_manager.vue")
          },
          {
            path:'/config_upload',
            name:'config_upload',
            meta:{
              permiss:EPermiss.config_upload,
            },
            component: ()=> import("../views/index/config_upload.vue")
          }
      ]
    },
    {
      path: '/player/z:zone_id/a:account_uid',
      name:'player',
      meta:{
        permiss:EPermiss.player_view,
      },
      component:()=> import("../views/player.vue"),
      
      children:[
        {
          path:'',
          name:'player_account1',
          meta:{
            permiss:EPermiss.account_info_view,
          },
          component:()=> import("../views/player/account.vue")
        },
        {
          path:'account',
          name:'player_info',
          meta:{
            permiss:EPermiss.account_info_view,
          },
          component:()=> import("../views/player/account.vue"),
          children:[
            {
              path:'',
              name:'account_info',
              meta:{
                permiss:EPermiss.account_info_view,
                tags_value:"info"
              },
              component:()=> import("../views/player/account/info.vue")
            },
            {
              path:'info',
              name:'account_info2',
              meta:{
                permiss:EPermiss.account_info_view,
                tags_value:"info"
              },
              component:()=> import("../views/player/account/info.vue")
            },
            {
              path:'log',
              name:'account_log',
              meta:{
                permiss:EPermiss.account_log_view,
                tags_value:"log"
              },
              component:()=> import("../views/player/account/log.vue")
            },
            {
              path:'log_pay',
              name:'account_log_pay',
              meta:{
                permiss:EPermiss.account_log_pay_view,
                tags_value:"log_pay"
              },
              component:()=> import("../views/player/account/log_pay.vue")
            },
          ]
        },
        {
          path:'r:role_uid',
          name:'role',
          meta:{
            permiss:EPermiss.role_view,
          },
          component:()=> import("../views/player/role.vue"),
          children:[
              {
                path:'',
                name:'role_base_attr1',
                meta:{
                  permiss:EPermiss.role_info_view,
                  tags_value:"base_attr"
                },
                component:()=> import("../views/player/role/base_attr.vue")
              },
              {
                path:'base_attr',
                name:'role_base_attr2',
                meta:{
                  permiss:EPermiss.role_info_view,
                  tags_value:"base_attr"
                },
                component:()=> import("../views/player/role/base_attr.vue")
              },
              {
                path:'equipment',
                name:'role_equipment',
                meta:{
                  permiss:EPermiss.role_equipment_view,
                  tags_value:"equipment"
                },
                component:()=> import("../views/player/role/equipment.vue")
              },
              {
                path:'backpack',
                name:'role_backpack',
                meta:{
                  permiss:EPermiss.role_backpack_view,
                  tags_value:"backpack"
                },
                component:()=> import("../views/player/role/backpack.vue")
              },
              {
                path:'warehouse',
                name:'role_warehouse',
                meta:{
                  permiss:EPermiss.role_warehouse_view,
                  tags_value:"warehouse"
                },
                component:()=> import("../views/player/role/warehouse.vue")
              },
              {
                path:'skill',
                name:'role_skill',
                meta:{
                  permiss:EPermiss.role_skill_view,
                  tags_value:"skill"
                },
                component:()=> import("../views/player/role/skill.vue")
              },
              {
                path:'mail',
                name:'role_mail',
                meta:{
                  permiss:EPermiss.role_mail_view,
                  tags_value:"mail"
                },
                component:()=> import("../views/player/role/mail.vue")
              },
              {
                path:'pet',
                name:'role_pet',
                meta:{
                  permiss:EPermiss.role_pet_view,
                  tags_value:"pet"
                },
                component:()=> import("../views/player/role/pet.vue")
              },
              {
                path:'title',
                name:'role_title',
                meta:{
                  permiss:EPermiss.role_title_view,
                  tags_value:"title"
                },
                component:()=> import("../views/player/role/title.vue")
              },
          ]
        },
      ]
    }

  ]
})


router.beforeEach((to,from,next)=>{
  let isLogin = localStorage.token ? true : false;
  const permiss = usePermissStore();
  if(isLogin)
  {
    const decode = jwt_decode(localStorage.token);
    if(decode.exp <= Math.round(new Date() / 1000))
    {
      isLogin = false;
    }else{
      permiss.handleSet(decode.identity);
      permiss.handleSetChannelId(decode.name);
    }
  }
  if (to.path == `/${LoginPath}` || to.path == "/register") {
    next();
  }else{
    if(isLogin)
    {
      if(to.meta.permiss == null)
      {
        console.log("next()");
        next();
      }
      else if(permiss.Ok(to.meta.permiss))
      {
        console.log("next()");
        next();
      }else{
        console.log("/403");
        next("/403");
      }
    }else{
      next(`/${LoginPath}`);
    }
  }
  console.log(to.path);
});


export default router
