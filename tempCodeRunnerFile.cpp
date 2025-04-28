#include<bits/stdc++.h>

#define ll long long
#define pb push_back

int mod = 1e9 + 7;

using namespace std;

struct segTree{
    vector<int> seg;
    segTree(int n){
        seg.resize(n*4);
    }

    void build(vector<int> &vec, int vertex, int tl, int tr){
        if(tl == tr){
            seg[vertex] = vec[tl];
        }else{
            int tm = (tl + tr)/2;
            build(vec, vertex*2, tl, tm);
            build(vec, vertex*2+1, tm+1, tr);
            seg[vertex] = seg[vertex*2] + seg[vertex*2+1];
        }
    }

    int query(int vertex, int tl, int tr, int l, int r){
        if(l > r){
            return 0;
        }else if(l == tl && r == tr){
            return seg[vertex];
        }else{
            int tm = (tl+tr)/2;
            return query(vertex*2, tl, tm, l, min(r, tm)) + query(vertex*2 + 1, tm + 1, tr, max(l, tm + 1), r);
        }
    }

    void update(int vertex, int tl, int tr, int pos, int val){
        if(tl == tr){
            seg[vertex] = val;
        }else{
            int tm = (tl+tr)/2;
            if(pos <= tm){
                update(2*vertex, tl, tm, pos, val);
            }else{
                update(2*vertex + 1, tm+1, tr, pos, val);
            }
            seg[vertex] = seg[vertex*2] + seg[vertex*2 + 1];
        }
    }
};

int main(){
    double x = 3*pow(2, 0.5) + 1/pow(2, 0.33);
    cout<<x<<endl;
    double y = 2*x*x - 5/x;
    cout<<y<<endl;
}