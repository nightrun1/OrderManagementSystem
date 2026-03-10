import Link from "next/link";

export default function Home() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-indigo-50">
      {/* Header */}
      <header className="border-b border-slate-200 bg-white/80 backdrop-blur-sm sticky top-0 z-10">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-gradient-to-br from-blue-600 to-indigo-600 rounded-lg flex items-center justify-center">
                <span className="text-white font-bold text-xl">O</span>
              </div>
              <h1 className="text-2xl font-bold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">Order Management</h1>
            </div>
            <div className="flex gap-3">
              <Link href="/login" className="px-4 py-2 text-sm font-medium text-slate-700 hover:text-blue-600 transition-colors">Login</Link>
              <Link href="/register" className="px-4 py-2 text-sm font-medium bg-gradient-to-r from-blue-600 to-indigo-600 text-white rounded-lg hover:shadow-lg hover:scale-105 transition-all">Sign Up</Link>
            </div>
          </div>
        </div>
      </header>

      {/* Hero Section */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
        <div className="text-center mb-16 space-y-4">
          <h2 className="text-5xl font-bold text-slate-900">Welcome to Your Store</h2>
          <p className="text-xl text-slate-600 max-w-2xl mx-auto">Manage your products, orders, and customers all in one place with our comprehensive platform.</p>
        </div>

        {/* Features Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-12">
          <Link href="/products" className="group relative bg-white rounded-2xl shadow-md hover:shadow-xl p-8 transition-all duration-300 hover:-translate-y-1 border border-slate-100">
            <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-blue-600 to-indigo-600 rounded-t-2xl"></div>
            <div className="w-12 h-12 bg-blue-100 rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
              <span className="text-2xl">📦</span>
            </div>
            <h3 className="text-xl font-bold text-slate-900 mb-2">Products</h3>
            <p className="text-slate-600">Browse and search through our product catalog</p>
          </Link>

          <Link href="/orders" className="group relative bg-white rounded-2xl shadow-md hover:shadow-xl p-8 transition-all duration-300 hover:-translate-y-1 border border-slate-100">
            <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-green-600 to-emerald-600 rounded-t-2xl"></div>
            <div className="w-12 h-12 bg-green-100 rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
              <span className="text-2xl">🛒</span>
            </div>
            <h3 className="text-xl font-bold text-slate-900 mb-2">Orders</h3>
            <p className="text-slate-600">View and manage your order history</p>
          </Link>

          <Link href="/dashboard" className="group relative bg-white rounded-2xl shadow-md hover:shadow-xl p-8 transition-all duration-300 hover:-translate-y-1 border border-slate-100">
            <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-purple-600 to-pink-600 rounded-t-2xl"></div>
            <div className="w-12 h-12 bg-purple-100 rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
              <span className="text-2xl">📊</span>
            </div>
            <h3 className="text-xl font-bold text-slate-900 mb-2">Dashboard</h3>
            <p className="text-slate-600">Overview of your account and activity</p>
          </Link>

          <Link href="/admin/products" className="group relative bg-white rounded-2xl shadow-md hover:shadow-xl p-8 transition-all duration-300 hover:-translate-y-1 border border-slate-100">
            <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-orange-600 to-red-600 rounded-t-2xl"></div>
            <div className="w-12 h-12 bg-orange-100 rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
              <span className="text-2xl">⚙️</span>
            </div>
            <h3 className="text-xl font-bold text-slate-900 mb-2">Admin Panel</h3>
            <p className="text-slate-600">Manage products and inventory (Admin only)</p>
          </Link>

          <Link href="/register" className="group relative bg-gradient-to-br from-blue-600 to-indigo-600 rounded-2xl shadow-md hover:shadow-xl p-8 transition-all duration-300 hover:-translate-y-1 text-white">
            <div className="w-12 h-12 bg-white/20 rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
              <span className="text-2xl">👤</span>
            </div>
            <h3 className="text-xl font-bold mb-2">Create Account</h3>
            <p className="text-blue-100">Join us and start managing your orders</p>
          </Link>

          <Link href="/login" className="group relative bg-gradient-to-br from-slate-700 to-slate-900 rounded-2xl shadow-md hover:shadow-xl p-8 transition-all duration-300 hover:-translate-y-1 text-white">
            <div className="w-12 h-12 bg-white/20 rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
              <span className="text-2xl">🔐</span>
            </div>
            <h3 className="text-xl font-bold mb-2">Sign In</h3>
            <p className="text-slate-300">Access your existing account</p>
          </Link>
        </div>
      </main>
    </div>
  );
}
