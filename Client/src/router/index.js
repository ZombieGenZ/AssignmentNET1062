import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "../stores/auth.store";

const HomePage = () => import("../pages/Home/HomePage.vue");
const LoginPage = () => import("../pages/Auth/LoginPage.vue");
const RegisterPage = () => import("../pages/Auth/RegisterPage.vue");

const ProductListPage = () => import("../pages/Product/ProductListPage.vue");
const ProductDetailPage = () => import("../pages/Product/ProductDetailPage.vue");

const ComboListPage = () => import("../pages/Combo/ComboListPage.vue");
const ComboDetailPage = () => import("../pages/Combo/ComboDetailPage.vue");

const CartPage = () => import("../pages/Cart/CartPage.vue");
const CheckoutPage = () => import("../pages/Cart/CheckoutPage.vue");

const VoucherPage = () => import("../pages/Voucher/VoucherPage.vue");

const OrderHistoryPage = () => import("../pages/Order/OrderHistoryPage.vue");
const OrderDetailPage = () => import("../pages/Order/OrderDetailPage.vue");

const ProfilePage = () => import("../pages/Account/ProfilePage.vue");
const LoginMethodsPage = () => import("../pages/Account/LoginMethodsPage.vue");

const AdminLayout = () => import("../pages/Admin/AdminLayout.vue");
const AdminDashboardPage = () => import("../pages/Admin/DashboardPage.vue");
const AdminOrderListPage = () => import("../pages/Admin/OrderListPage.vue");
const AdminOrderDetailPage = () => import("../pages/Admin/OrderDetailPage.vue");
const AdminCategoryPage = () => import("../pages/Admin/CategoryListPage.vue");
const AdminProductPage = () => import("../pages/Admin/ProductListPage.vue");
const AdminComboPage = () => import("../pages/Admin/ComboListPage.vue");
const AdminVoucherPage = () => import("../pages/Admin/VoucherListPage.vue");

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", name: "home", component: HomePage },
    { path: "/login", name: "login", component: LoginPage },
    { path: "/register", name: "register", component: RegisterPage },

    { path: "/products", name: "products", component: ProductListPage },
    { path: "/products/:id", name: "product-detail", component: ProductDetailPage },

    { path: "/combos", name: "combos", component: ComboListPage },
    { path: "/combos/:id", name: "combo-detail", component: ComboDetailPage },

    { path: "/cart", name: "cart", component: CartPage, meta: { requiresAuth: true } },
    { path: "/checkout", name: "checkout", component: CheckoutPage, meta: { requiresAuth: true } },

    { path: "/vouchers", name: "vouchers", component: VoucherPage },

    { path: "/orders", name: "orders", component: OrderHistoryPage, meta: { requiresAuth: true } },
    { path: "/orders/:id", name: "order-detail", component: OrderDetailPage, meta: { requiresAuth: true } },

    { path: "/account/profile", name: "profile", component: ProfilePage, meta: { requiresAuth: true } },
    { path: "/account/login-methods", name: "login-methods", component: LoginMethodsPage, meta: { requiresAuth: true } },

    {
      path: "/admin",
      component: AdminLayout,
      meta: { requiresAuth: true, requiresAdmin: true },
      children: [
        { path: "", name: "admin-dashboard", component: AdminDashboardPage },
        { path: "orders", name: "admin-orders", component: AdminOrderListPage },
        { path: "orders/:id", name: "admin-order-detail", component: AdminOrderDetailPage },
        { path: "categories", name: "admin-categories", component: AdminCategoryPage },
        { path: "products", name: "admin-products", component: AdminProductPage },
        { path: "combos", name: "admin-combos", component: AdminComboPage },
        { path: "vouchers", name: "admin-vouchers", component: AdminVoucherPage },
      ],
    },
  ],
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return next({ name: "login", query: { redirect: to.fullPath } });
  }

  if (to.meta.requiresAdmin && !authStore.isAdmin) {
    return next({ name: "home" });
  }

  next();
});

export default router;
